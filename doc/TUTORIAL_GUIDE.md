# 📘 ASP.NET Core 8.0 MVC + Render Docker 雙環境與 GitHub 治理閉環標準實戰手冊

> **本教學檔為單一權威參考文件 (Single Source of Truth)。** 所有環境建置、雲端部署、權限治理與 Issue ➔ PR 演練步驟皆完整收錄於此，後續無需重複說明，隨時點擊各章節查閱即可。

---

## 📑 目錄
1. [系統架構與技術棧規格](#1-系統架構與技術棧規格)
2. [本地端開發與驗證指南](#2-本地端開發與驗證指南)
3. [Render 雲端 Docker 雙環境自動發布 (Blueprint)](#3-render-雲端-docker-雙環境自動發布-blueprint)
4. [GitHub 團隊協作與權限治理防護 (CODEOWNERS & Rulesets)](#4-github-團隊協作與權限治理防護-codeowners--rulesets)
5. [第一個功能實戰：Issue ➔ PR 閉環與行號審查演練 SOP](#5-第一個功能實戰issue--pr-閉環與行號審查演練-sop)
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
- 健康檢查探針端點：`http://localhost:5051/healthz`

---

## 3. Render 雲端 Docker 雙環境自動發布 (Blueprint)

專案已內建 [`render.yaml`](../render.yaml) Infrastructure as Code (IaC) 藍圖檔，宣告兩套獨立的服務：

| 環境名稱 | 對應 Git 分支 | Render 服務名稱 | 觸發時機 |
| :--- | :--- | :--- | :--- |
| **正式機 (Production)** | `main` | `inventory-flow-prod` | 主管核准 PR 並 Merge 後自動發布 |
| **測試機 (Staging)** | `develop` | `inventory-flow-staging` | 內部開發分支 Push 自動發布 |
| **PR 預覽環境 (Previews)** | `feature/*` | `inventory-flow-pr-<編號>` | 提出 PR 時自動生成臨時網址，Merge 後自動銷毀 |

### 3.1 首次推送到 GitHub
請在 GitHub 上建立一個新的公開或私有儲存庫（如 `MvcRenderApp`），並於本機執行：
```bash
# 綁定 GitHub 遠端倉庫 (請將 <YOUR-GITHUB-URL> 替換為您的倉庫網址)
git remote add origin <YOUR-GITHUB-URL>

# 推送 main 分支至 GitHub
git push -u origin main

# 建立並推送 develop 測試分支
git checkout -b develop
git push -u origin develop
git checkout main
```

### 3.2 在 Render 上一鍵套用 Blueprint
1. 登入 [Render 控制台](https://dashboard.render.com/)。
2. 點擊右上角 **New +** ➔ 選擇 **Blueprint**。
3. 連結您的 GitHub 帳號並選取本專案倉庫。
4. Render 會自動讀取專案根目錄下的 `render.yaml`，並自動為您建立 `inventory-flow-prod` 與 `inventory-flow-staging` 兩個服務。
5. 點擊 **Apply**，Render 將開始執行遠端 Docker 多階段構建並發布上線！

---

## 4. GitHub 團隊協作與權限治理防護 (CODEOWNERS & Rulesets)

為了防止開發人員繞過審查直接將代碼推送到 `main`，必須在 GitHub 上配置權限門禁：

### 4.1 配置 Branch Rulesets（保護 main 與 develop）
1. 進入 GitHub 倉庫 ➔ **Settings** ➔ **Rules** ➔ **Rulesets** ➔ 點擊 **New ruleset**。
2. Ruleset Name 填寫：`Production Protection`。
3. Enforcement 選擇 **Active**。
4. Target branches 點擊 **Add target** ➔ **Include default branch (main)**。
5. 勾選關鍵規則：
   - ✅ **Require a pull request before merging**：
     - 勾選 **Require approvals**：設定至少 `1` 位審查者核准。
     - 勾選 **Dismiss stale pull request approvals when new commits are pushed**（有新 Commit 時需重新審查）。
     - 勾選 **Require review from Code Owners**（強制需由 CODEOWNERS 名單內的主管 Approve）。
   - ✅ **Require status checks to pass**：
     - 搜尋並勾選我們的 CI 工作流 `build`（確保 `dotnet build` 通過才放行）。
   - ✅ **Block force pushes**（禁止強推覆蓋歷史）。
6. 點擊 **Save changes**。

### 4.2 CODEOWNERS 審查機制
專案已內建 [`.github/CODEOWNERS`](../.github/CODEOWNERS)：
```text
* @zhangs1124
```
當任何 PR 發起時，GitHub 會自動指派 `@zhangs1124` 為 Mandatory Reviewer，作者本人**絕對無法自己審核自己**。

---

## 5. 第個功能實戰：Issue ➔ PR 閉環與行號審查演練 SOP

以下為標準的敏捷開發閉環作業流程：

### 步驟 A：主管開立 Issue 工單 (What & Why)
1. 前往 GitHub 倉庫 ➔ **Issues** ➔ **New issue** ➔ 選擇 **🚀 功能需求工單 (Feature Request)**。
2. 標題填入：`[Feature] 實作物料清單查詢與 Modal 快速入庫/出庫`。
3. 內容填入驗收規格（例如：在側邊欄啟用物料清單、點擊按鈕彈出 Modal 異動庫存、儲存後無刷新更新）。
4. 點擊 **Submit new issue**，取得工單編號（例如 `#1`）。

### 步驟 B：工程師開分支並開發 (How)
```bash
# 從最新 main 切出功能分支
git checkout main
git pull
git checkout -b feature/inventory-modal-crud

# 開始撰寫程式碼 (Modal CRUD、Partial View、AJAX 提交)...

# 提交變更並推送至分支
git add .
git commit -m "feat: 實作物料庫存清單與 Modal 快速入庫/出庫"
git push -u origin feature/inventory-modal-crud
```

### 步驟 C：提出 PR (Pull Request) 並連動 Issue
1. 在 GitHub 頁面點擊 **Compare & pull request**。
2. 系統會自動帶入專案預設的 [`.github/pull_request_template.md`](../.github/pull_request_template.md)。
3. 在第一行填入：`Closes #1`（**關鍵：此標籤會讓主管 Merge 時自動關閉 Issue #1**）。
4. 勾選自我檢查清單，並點擊 **Create pull request**。

### 步驟 D：觸發 CI 與 Render 預覽環境 (PR Previews)
- GitHub Actions 自動執行 CI，確認程式碼編譯無誤（綠色打勾）。
- Render 自動針對該 PR 啟動臨時容器並提供預覽網址（例如：`https://inventory-flow-pr-1.onrender.com`）。

### 步驟 E：主管審查 (Code Review) 與行號留言
1. 主管點擊 PR 的 **Files changed** 標籤頁。
2. 針對有疑問或讚賞的具體程式碼行號，點擊 **`+`** 號發起行號留言（Inline Comment）。
3. 雙方針對行號討論，修正後點擊 **Resolve conversation**。
4. 主管在右上角點擊 **Review changes** ➔ 選擇 **Approve**。

### 步驟 F：Merge 放行與自動結案 (Audit Trail)
1. 主管點擊 **Merge pull request** ➔ **Confirm merge**。
2. GitHub 自動將代碼合入 `main`。
3. **Issue #1 自動轉為 Closed (已完成)**。
4. Render 自動觸發 Production 正式機部署，並自動銷毀剛才的臨時 PR 預覽機器。

---

## 6. 常見維運問題與排查清單 (Troubleshooting)

1. **Render 部署後出現 502 Bad Gateway？**
   - **原因**：容器未正確綁定 Render 動態指派的 `$PORT`。
   - **解法**：確認 `Dockerfile` 的啟動指令包含 `--urls http://0.0.0.0:${PORT:-8080}`（本專案已內建）。
2. **健康探針逾時？**
   - **原因**：`/healthz` 路由未被正常註冊或資料庫連線卡住。
   - **解法**：確認 `Program.cs` 中的 `app.MapGet("/healthz", ...)` 存在，本專案健康探針不依賴外部連線，保證極速響應。
3. **Render 免費版資料庫重啟重設？**
   - **說明**：Render 免費容器屬於 Ephemeral（無狀態）。重啟時會自動重新執行 `EnsureCreated()` 載入 6 筆標準種子資料，這對「驗收演練與 CI 測試」反而是極大優點，永遠乾淨不髒。若日後需要永久留存，可在 Render 上掛載 Persistent Disk 或連線外部 PostgreSQL。
