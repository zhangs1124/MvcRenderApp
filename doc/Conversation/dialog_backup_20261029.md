# 對話與專案進度備份紀錄 - 2026/10/29

## 📌 任務概要
- **專案名稱**：GITHubAction1 (https://github.com/zhangs1124/GithubAction1)
- **主題**：深入探討團隊協作治理、Issue 與 PR 閉環、多人審核權限控管、雙環境（測試機 vs 正式機）部署機制，以及 Render Docker 雲端部署最佳實踐。

---

## 💡 今日核心架構結論與關鍵知識點

### 1. Issue 與 PR 的團隊分工與角色
- **Issue（工單）**：主管 / PM / QA 於開會後開立，定義「要做什麼（What & Why）」，具備任務編號（如 #3）、優先級與指派人。
- **PR（合併請求）**：工程師開分支寫 Code 與測試，提出審核（How）。
- **Closes #Issue編號 連動**：PR 說明寫入 Closes #Issue編號，於主管 Merge 瞬間，GitHub 自動將該 Issue 結案（Closed），形成完整追蹤鏈（Audit Trail）。

### 2. PR 審核與合併權限治理（多人安全控管）
- **核心原則**：開發者（作者）絕對不可自己審核或直接 Merge 自己的 PR。
- **權限防護設定**：
  1. Rulesets 勾選 Require a pull request before merging。
  2. 勾選 Require approvals（至少 1~2 人審核，作者不能審自己）。
  3. 配置 .github/CODEOWNERS，指定只有特定主管 / 架構師點擊 Approve 才能放行 Merge。

### 3. 多環境預發布驗收（測試機 vs 正式機）
- **模式 A：Render PR Previews（臨時預覽環境）**：
  - PR 提出時，Render 自動針對該 PR 建立專屬臨時測試網址（如 https://myapp-pr-5.onrender.com）。
  - 主管與 QA 在測試網址驗收無誤後才點擊 Merge。
  - Merge 後自動發布至正式機，並自動銷毀臨時測試機。
- **模式 B：雙分支 Staging 架構（develop 測試 ➔ main 正式）**：
  - 傳統固定伺服器架構，develop 分支部署測試環境，main 分支部署正式環境。

### 4. 團隊溝通與審核最佳實踐
- **黃金法則**：詳細代碼討論 100% 留在 GitHub PR（行號留言 Inline Comments、待辦對話 Resolve Conversation、歷史查驗），通訊軟體（Teams / Slack / LINE）僅作為門鈴式通知。

---

## 🚀 下一階段新專案籌備（ASP.NET Core MVC + Docker + Render）
1. **專案規劃**：新建專屬 ASP.NET Core MVC 獨立專案。
2. **容器化**：配置多階段最佳化 Dockerfile，確保兼顧建置效能與最小映像檔體積。
3. **Render 部署規劃**：
   - 測試環境（Staging / PR Previews）
   - 正式環境（Production）
4. **DevOps 流程**：完整落實 Issue 工單指派 ➔ 功能分支開發 ➔ CI 自動測試 ➔ 測試機驗收 ➔ 主管 Approve ➔ 正式機 CD 自動發布。
