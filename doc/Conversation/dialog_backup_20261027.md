# 對話與專案進度備份紀錄 - 2026/10/27

## 📌 任務概要
- **專案名稱**：GITHubAction1 (https://github.com/zhangs1124/GithubAction1)
- **目標**：從無到有在 GitHub 實戰 GitHub Actions (CI/CD)、PR (Pull Request) 協作流程、分支保護（Branch Protection Rulesets），以及本地 Self-Hosted Runner 與 IIS 自動化部署規劃。

---

## 🛠️ 已建立與核心變更檔案
1. [src/calculator.py](file:///d:/project/GITHubAction1/src/calculator.py)：核心計算機邏輯（加法、除法、乘法、減法）。
2. [	ests/test_calculator.py](file:///d:/project/GITHubAction1/tests/test_calculator.py)：pytest 單元測試案例。
3. [
equirements.txt](file:///d:/project/GITHubAction1/requirements.txt)：專案相依套件清單 (pytest>=7.0.0)。
4. [.github/workflows/ci.yml](file:///d:/project/GITHubAction1/.github/workflows/ci.yml)：CI 自動化測試工作流程（監聽 push & PR，注入 PYTHONPATH: .）。
5. [index.html](file:///d:/project/GITHubAction1/index.html)：Web 版線上計算機 UI 介面。
6. [.github/workflows/deploy.yml](file:///d:/project/GITHubAction1/.github/workflows/deploy.yml)：CD 持續部署腳本（成功自動發布至 GitHub Pages）。
7. [.github/workflows/local-deploy.yml](file:///d:/project/GITHubAction1/.github/workflows/local-deploy.yml)：本地 Self-Hosted Runner 自動部署工作流程。

---

## 💡 重要知識點與排查結論

### 1. CI 與虛擬機模組搜尋路徑排查
- **問題**：在 GitHub-hosted runner (Ubuntu) 執行 pytest 時拋出 ModuleNotFoundError: No module named 'src'（Exit code 2）。
- **根因**：Linux 環境下若未配置 PYTHONPATH，pytest 收集測試時不會自動將專案根目錄納入路徑。
- **解法**：在 ci.yml 測試步驟中注入環境變數 env: PYTHONPATH: .，並建立 src/__init__.py。

### 2. PR (Pull Request) 與 Git 分支本質
- **PR 追蹤機制**：PR 綁定分支指針（HEAD），在該分支上持續 push 會自動追加 commit 並觸發重新測試，無需重複發起 PR。
- **分支刪除觀念**：刪除分支僅是移除便利貼標籤，已 Merge 進 main 的 commit 歷史長河與線條永久留存。

### 3. 分支保護（Branch Protection / Rulesets）實彈驗證
- **保護政策**：main 分支禁止直接 push，強制發起 PR，且強制 	est 檢查必須成功。
- **驗證**：故意推入減法錯誤程式碼（ + b），觸發 CI 紅叉叉，GitHub 成功鎖死 Merge pull request 按鈕（標記 Required）；修復為  - b 後自動重測通過，按鈕自動解鎖。

### 4. CD（持續部署）成功落地
- **GitHub Pages CD**：PR 合併進 main 後自動觸發 deploy.yml，成功上線正式公開網站：
  https://zhangs1124.github.io/GithubAction1/。

### 5. Self-Hosted Runner 與本地 / 內網 IIS 部署架構
- **突破內網防火牆**：Runner 採用由內向外 Long-polling 連線，免固定公網 IP、免開防火牆連接埠。本機 4W-7000-6-53 成功註冊並保持 online。
- **IIS 零鎖檔發布四部曲**：
  1. 放置 pp_offline.htm（IIS 自動釋放 w3wp.exe 檔案鎖定並顯示優雅維護頁）。
  2. 備份前版至備份目錄。
  3. 覆蓋更新發布檔（排除本機專屬 Web.config 防止覆蓋連線字串）。
  4. 移除 pp_offline.htm 並重啟 AppPool，以 HTTP Health Check (200 OK) 驗收。

---

## 🎯 下一步任務
1. 練習將此架構推廣至企業級 ASP.NET MVC / Core 專案。
2. 規劃多環境分流部署（develop 分支部署測試機 IIS，main 分支部署正式機 IIS）。
3. 配置 Web.config 轉換（Transformation）與 Secrets 保護。
