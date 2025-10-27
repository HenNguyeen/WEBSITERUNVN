let sidemen = document.getElementById('sidemenu');

function closemenu() {
    sidemen.style.right = '0%';
}

function openemenu() {
    sidemen.style.right = '100%';
}
// Lấy phần tử
const btn = document.getElementById("dropdownBtn");
const menu = document.getElementById("dropdownMenu");

// Toggle menu khi click nút
btn.addEventListener("click", function (e) {
    e.stopPropagation(); // Ngăn chặn nổi bọt
    menu.classList.toggle("show");
});

// Đóng khi click ra ngoài
document.addEventListener("click", function () {
    menu.classList.remove("show");
});
