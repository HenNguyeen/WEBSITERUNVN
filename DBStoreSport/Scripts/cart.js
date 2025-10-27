<script>
    document.addEventListener("DOMContentLoaded", function () {
        loadCartUI();
});

    function loadCartUI() {
        fetch("/ShoppingCart/GetCartJson")
            .then(res => res.json())
            .then(data => {
                const cartContent = document.querySelector(".cart-content");
                const totalPrice = document.querySelector(".total-price");

                // Xóa nội dung cũ
                cartContent.innerHTML = "";

                if (data.Items.length === 0) {
                    cartContent.innerHTML = "<p>Giỏ hàng đang trống.</p>";
                    totalPrice.textContent = "0 $";
                    return;
                }

                // Tạo nội dung mới từ từng item
                data.Items.forEach(item => {
                    const itemDiv = document.createElement("div");
                    itemDiv.classList.add("cart-box");
                    itemDiv.innerHTML = `
                    <img src="/Content/images/${item.ProductId}.jpg" alt="${item.Name}" class="cart-img">
                    <div class="detail-box">
                        <div class="cart-product-title">${item.Name}</div>
                        <div class="cart-price">${item.Price} $</div>
                        <input type="number" value="${item.Quantity}" class="cart-quantity" disabled>
                    </div>
                    <i class='bx bxs-trash-alt cart-remove'></i>
                `;
                    cartContent.appendChild(itemDiv);
                });

                // Tổng tiền
                totalPrice.textContent = data.TotalPrice + " $";
            })
            .catch(error => {
                console.error("Lỗi tải giỏ hàng:", error);
            });
}
</script>
