# 📘 ASP.NET Core 8.0 MVC + Render Docker 雙環境與 GitHub 治理閉環標準實戰手冊

> **本教學檔為單一權威參考文件 (Single Source of Truth)。** 所有環境建置、雲端部署、權限治理與 Issue ➔ PR 演練步驟皆完整收錄於此，後續無需重複說明，隨時點擊各章節查閱即可。

---

## 📑 目錄
1. [系統架構與技術棧規格](#1-系統架構與技術棧規格)
2. [本地端開發與驗證指南](#2-本地端開發與驗證指南)
3. [Render 雲端 Docker 雙環境自動發布 (Blueprint)](#3-render-雲端-docker-雙環境自動發布-blueprint)
4. [GitHub 團隊協作與權限治理防護 (CODEOWNERS & Rulesets)](#4-github-團隊協作與權限治理防護-codeowners--rulesets)
5. [實戰經典：Issue ➔ 測試機 PR ➔ 正式機 Release 閉環全流程](#5-實戰經典issue--測試機-pr--正式機-release-閉環全流程)
6. [常見維運問題與排查清單 (Troubleshooting)](#6-常見維運問題與排查清單-troubleshooting)

---

## 1. 系統架構與技術棧規格

* **後端核心**：ASP.NET Core 8.0 (Model-View-Controller)
* **資料庫**：SQLite + Entity Framework Core 8.0（內建種子資料 Seed Data，開箱即用）
* **容器化**：多階段最佳化 `Dockerfile`（SDK 建置 ➔ 輕量 Runtime，相容 Render 動態 `$PORT`）
* **雲端平台**：Render Cloud (Free Web Service + Blueprint `render.yaml`)
* **編碼規格**：全面強制 **UTF-8 (含 BOM)**，徹底防範跨平台亂碼
* **前端規範**：遵循 SPA-like、Modal-First CRUD、響應式表格防擠壓標準（`white-space: nowrap` + `min-width: 850px`）

---

## 2. 本地端開發與驗證指南

### 2.1 本地啟動專案
```bash
# 還原依賴與編譯專案
dotnet build

# 啟動本機開發伺服器 (預設監聽 http://localhost:5051)
dotnet run --launch-profile http
```

### 2.2 本地存取端點
- 系統首頁儀表板：`http://localhost:5051/`
- 物料庫存管理：`http://localhost:5051/Inventory`
- 健康檢查探針端點：`http://localhost:5051/healthz`

---

## 3. Render 雲端 Docker 雙環境自動發布 (Blueprint)

專案已內建 [`render.yaml`](../render.yaml) Infrastructure as Code (IaC) 藍圖檔，宣告兩套獨立的服務：

| 環境名稱 | 對應 Git 分支 | Render 服務名稱 | 觸發時機 |
| :--- | :--- | :--- | :--- |
| **正式機 (Production)** | `main` | `inventory-flow-prod` | `main` 分支更新時 Auto-Deploy |
| **測試機 (Staging)** | `develop` | `inventory-flow-staging` | `develop` 分支更新時 Auto-Deploy |

### 3.1 首次推送到 GitHub
```bash
# 綁定 GitHub 遠端倉庫
git remote add origin https://github.com/zhangs1124/MvcRenderApp.git
git push -u origin main
git checkout -b develop
git push -u origin develop
```

### 3.2 在 Render 上一鍵套用 Blueprint
1. 登入 [Render 控制台](https://dashboard.render.com/)。
2. 點擊右上角 **New +** ➔ 選擇 **Blueprint**。
3. 選取 `MvcRenderApp` 倉庫 ➔ 點擊 **Connect**。
4. Render 自動讀取 `render.yaml`，一次辨識出 `inventory-flow-prod` 與 `inventory-flow-staging` 兩台機器。
5. 填寫 Blueprint Name（如 `mvcrenderapp`）➔ 點擊 **Deploy Blueprint**，雲端立即並行建置兩台獨立容器！

---

## 4. GitHub 團隊協作與權限治理防護 (CODEOWNERS & Rulesets)

* **`.github/pull_request_template.md`**：內建 `Closes #ID` 自動結案關鍵字與 RWD / UTF-8 自檢清單。
* **`.github/CODEOWNERS`**：指定審核主管 `@zhangs1124`。
* **`.github/workflows/ci.yml`**：每次 Push 或 PR 時，GitHub Actions 自動在 12 秒內執行 `dotnet build` 確保品質。

---

## 5. 實戰經典：Issue ➔ 測試機 PR ➔ 正式機 Release 閉環全流程

以下為今天實戰演練的標準生命週期：

### 階段一：主管開立需求工單 (What & Why)
1. 進入 GitHub ➔ **Issues** ➔ **New issue** ➔ 選擇「🚀 功能需求工單」。
2. 填寫標題 `[Feature] 實作物料清單查詢與 Modal 快速入庫/出庫` 與驗收標準，點擊 **Create** 產出 **`Issue #1`**。
3. ★ **核心觀念**：Issue 屬於整個專案大樓，不需要也不綁定特定分支。

### 階段二：工程師分支開發 (How)
1. 從測試分支切出小工班分支：`git checkout -b feature/issue-1-inventory-crud`。
2. 實作 Controller、Views、Partial View Modal 與 RWD 表格。
3. 編譯通過後推送：`git push -u origin feature/issue-1-inventory-crud`。

### 階段三：工程師遞交 PR #2 (進測試機 Staging)
1. 在 GitHub 建立 PR：
   - **`base: develop`**（目的地：測試機）
   - **`compare: feature/issue-1-inventory-crud`**（來源：功能分支）
   - 內容填寫：`Closes #1`。
2. 點擊 **Create pull request** 送出！
3. GitHub Actions 機器人 12 秒自動通過 CI（綠勾 ✅）。

### 階段四：主管審查與測試機驗收
1. 主管點進 **Files changed** 進行行號審查，並在下方發表主管留言評語（如：*「經主管審查，出入庫邏輯防呆清晰，予以核准！」*）。
2. 主管點擊 **Merge pull request** ➔ **Confirm merge**。
3. **成果**：代碼進入 `develop`，Render 測試機 (`inventory-flow-staging`) 自動部署上線，此時正式機毫髮無傷（雙環境隔離）！

### 階段五：正式機發布 Release PR #3 (推向 Production)
1. 測試機滿意後，主管建立發布 PR：
   - **`base: main`**（目的地：正式機）
   - **`compare: develop`**（來源：測試機）
   - 內容填寫：`Closes #1`。
2. 主管點擊 **Merge pull request** ➔ **Confirm merge**。
3. **終極連鎖反應**：
   - **Issue #1 在這一刻正式由 GitHub 蓋章 Closed 自動結案！**
   - **Render 正式機 (`inventory-flow-prod`) 觸發 Auto-Deploy 自動部署！**
   - 正式機黃色「演練預備」消失，物料管理功能正式在正式機全面營運！

---

## 6. 常見維運問題與排查清單 (Troubleshooting)

1. **為什麼 Issue #1 在合併進 develop 時沒結案？**
   - GitHub 官方規則：`Closes #ID` 只有在代碼合併進**預設分支 (`main`)** 時才會自動關閉工單。合入 `develop` 代表仍在測試階段，因此保持 Open，直到 Release 合入 `main` 才正式結案。
2. **Render 免費版冷啟動延遲？**
   - 免費實例若閒置 15 分鐘會自動休眠，首次喚醒約需 50 秒。重新整理即可正常使用。
3. **如何確保中文字不亂碼？**
   - 專案檔案強制使用 UTF-8 (含 BOM) 編碼，可執行 `@fix-file-encoding-bom` 批次轉換。
