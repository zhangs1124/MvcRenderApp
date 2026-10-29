/* ==========================================================================
   庫存雲端管家 - 全域通用 JS 行為 (SPA-like Modal + 側邊欄收合)
   ========================================================================== */

document.addEventListener("DOMContentLoaded", function () {
    // 側邊導覽列收摺控制
    const sidebar = document.getElementById("sidebar");
    const sidebarCollapseBtn = document.getElementById("sidebarCollapse");

    if (sidebar && sidebarCollapseBtn) {
        sidebarCollapseBtn.addEventListener("click", function () {
            sidebar.classList.toggle("collapsed");
        });
    }
});

/**
 * 全域 SPA 通用 Modal 載入器 (Modal-First CRUD 標準規範)
 * @param {string} url - 欲透過 AJAX 取得 Partial View 的 Action 網址
 * @param {string} title - 彈窗標題
 * @param {string} size - 彈窗尺寸 ('sm', 'lg', 'xl'，預設為普通大小)
 */
function openGlobalModal(url, title, size = "") {
    const modalEl = document.getElementById("globalModal");
    const modalDialog = document.getElementById("globalModalDialog");
    const modalTitle = document.getElementById("globalModalTitle");
    const modalBody = document.getElementById("globalModalBody");

    if (!modalEl || !modalBody) return;

    // 設定標題
    if (modalTitle) {
        modalTitle.innerHTML = `<i class="bi bi-window me-2 text-primary"></i> ${title}`;
    }

    // 調整彈窗大小
    modalDialog.className = "modal-dialog modal-dialog-centered";
    if (size) {
        modalDialog.classList.add(`modal-${size}`);
    }

    // 初始 Loading 狀態
    modalBody.innerHTML = `
        <div class="text-center py-5">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">載入中...</span>
            </div>
            <div class="text-muted mt-2">正在載入模組，請稍候...</div>
        </div>
    `;

    const bsModal = new bootstrap.Modal(modalEl);
    bsModal.show();

    // AJAX 抓取 Partial View
    fetch(url, { credentials: "same-origin" })
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP 錯誤碼: ${response.status}`);
            }
            return response.text();
        })
        .then(html => {
            modalBody.innerHTML = html;
        })
        .catch(err => {
            modalBody.innerHTML = `
                <div class="alert alert-danger my-3">
                    <i class="bi bi-exclamation-triangle-fill me-2"></i> 模組載入失敗：${err.message}
                </div>
            `;
        });
}
