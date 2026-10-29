# 對話與專案進度備份紀錄 - 2026/10/29

## 📌 任務概要
- **專案名稱**：MvcRenderApp (https://github.com/zhangs1124/MvcRenderApp)
- **主題**：從零建立 ASP.NET Core 8.0 MVC 庫存管理系統，完成版面設計、SQLite 資料庫、Render Docker 多階段容器化與雙環境 (Staging / Production) 配置，並完整實戰演練 GitHub Issue ➔ 功能分支 ➔ CI 自動化 ➔ PR 行號審查 ➔ Merge 流程。

---

## 💡 今日核心成果與落地實踐

### 1. 系統基底架構完成
- **架構**：ASP.NET Core 8.0 MVC + SQLite + EF Core 8.0，內建標準物料種子資料 (Seed Data)。
- **版面**：套用 `@mvc-sidebar-layout` 企業深藍側邊欄，模組化 Partial View (`_Sidebar`, `_Header`, `_Footer`, `_GlobalModal`)。
- **編碼規範**：全專案強制 UTF-8 (含 BOM) 編碼，通過 `@fix-file-encoding-bom` 檢驗。
- **雲端健康探針**：實作 `/healthz` 端點，回傳 HTTP 200 JSON 狀態。

### 2. Render Docker 雙環境 (Infrastructure as Code)
- **多階段 Dockerfile**：SDK 還原層快取 ➔ 輕量 ASP.NET Runtime，安全非 root 使用者，動態相容 Render `$PORT`。
- **render.yaml 藍圖**：
  - 正式機 (Production)：`inventory-flow-prod` 連動 `main` 分支。
  - 測試機 (Staging)：`inventory-flow-staging` 連動 `develop` 分支。

### 3. 單一權威教學手冊 (Single Source of Truth)
- 產出 `doc/TUTORIAL_GUIDE.md`，後續所有指令與雙環境串接 SOP 一律查閱此檔，無需重述。

### 4. 首次實戰演練：Issue ➔ PR 閉環大成功！
- **主管開立工單**：在 GitHub 開立 `Issue #1: [Feature] 實作物料清單查詢與 Modal 快速入庫/出庫`。
- **工程師接棒開發**：
  - 切出功能分支 `feature/issue-1-inventory-crud`。
  - 實作 `InventoryController.cs`、`Views/Inventory/Index.cshtml`、`_AdjustStockModal.cshtml`。
  - 遵循 SPA-like Modal-First CRUD 規範，AJAX 無刷新異動庫存，表格符合 RWD 防擠壓標準。
- **提出 PR 與 CI 自動驗證**：
  - 提出 `PR #2`，設定 `base: develop`，關聯 `Closes #1`。
  - GitHub Actions CI 在 12 秒內自動跑完 `dotnet build` 通過驗證 (綠勾 ✅)。
- **主管審查與放行**：
  - 主管進行代碼審查與 Comment 留言互動。
  - 主管點擊 `Confirm merge` 成功將代碼合入 `develop` 分支！

### 5. 關鍵機制認知收穫
- **為什麼 Issue 屬於整個專案？**：因為 Issue 是高層次的需求 (What & Why)，不綁定單一分支。
- **develop vs main 的職責分工**：
  - `develop` 是內部測試機 (Staging)，隨便測、不怕爆。
  - `main` 是正式營運機 (Production)，只有在測試機確認 100% 完美時，才發起 Release PR 合入。
- **GitHub 結案機制**：`Closes #1` 在合入預設分支 (`main`) 時會自動將 Issue 標記為 Closed。合入 `develop` 時則建立關聯與進度追蹤！
