// xử handleClick user
const user = document.querySelector(".user")
const iconEye = document.querySelector(".iconEye")
const inputPassWord = document.getElementById("inputMatKhau")

function handleClickUser(event) {
    event.stopPropagation(); // Ngăn sự kiện nhấp lan truyền ra ngoài

    if (user.classList.contains("visible")) {
        user.classList.remove("visible")
    } else {
        user.classList.add("visible")
    }
}

document.addEventListener("click", function (e) {
    if (user && !user.contains(e.target)) {
        user.classList.remove("visible")
    }
})

function handleClickChangeEye() {
    if (iconEye.classList.contains("fa-eye-slash")) {
        iconEye.classList.add("fa-eye");
        iconEye.classList.remove("fa-eye-slash");
        inputPassWord.type = "text";
    } else {
        iconEye.classList.remove("fa-eye");
        iconEye.classList.add("fa-eye-slash");
        inputPassWord.type = "password";
    }
}

// xử lý toggle menu
function toggleSubMenu(submenuId) {
    var allSubMenus = document.querySelectorAll('.subMenu');

    for (var i = 0; i < allSubMenus.length; i++) {
        var menu = allSubMenus[i];
        if (menu.id !== submenuId) {
            menu.classList.remove("open");
            menu.parentElement.querySelector(".icon_1").classList.add("icon__visible");
            menu.parentElement.querySelector(".icon_1").classList.remove("icon__hidden");
            menu.parentElement.querySelector(".icon_2").classList.add("icon__hidden");
            menu.parentElement.querySelector(".icon_2").classList.remove("icon__visible");
        }
    }

    var subMenu = document.getElementById(submenuId);

    if (!subMenu.classList.contains("open")) {
        subMenu.classList.add("open"); // Mở menu được nhấp vào
        subMenu.parentElement.querySelector(".icon_1").classList.add("icon__hidden");
        subMenu.parentElement.querySelector(".icon_1").classList.remove("icon__visible");
        subMenu.parentElement.querySelector(".icon_2").classList.add("icon__visible");
        subMenu.parentElement.querySelector(".icon_2").classList.remove("icon__hidden");
    } else {
        subMenu.classList.remove("open"); // Đóng menu được nhấp vào
        subMenu.parentElement.querySelector(".icon_1").classList.add("icon__visible");
        subMenu.parentElement.querySelector(".icon_1").classList.remove("icon__hidden");
        subMenu.parentElement.querySelector(".icon_2").classList.add("icon__hidden");
        subMenu.parentElement.querySelector(".icon_2").classList.remove("icon__visible");
    }
}

document.getElementById("myForm").addEventListener("submit", function (event) {
    const errTenDangNhap = document.querySelector(".errTenDangNhap");
    const errMatKhau = document.querySelector(".errMatKhau");
    const errEmail = document.querySelector(".errEmail");
    const errHoTen = document.querySelector(".errHoTen");
    const errNhomNguoiDung = document.querySelector(".errNhomNguoiDung")

    const inputTenDangNhap = document.getElementById("TenDangNhap");
    const inputMatKhau = document.getElementById("MatKhau");
    const inputEmail = document.getElementById("Email");
    const inputHoTen = document.getElementById("HoTen");
    const inputMaNhom = document.getElementById("MaNhom");

    // Reset lỗi trước khi validate
    errTenDangNhap.innerText = "";
    errMatKhau.innerText = "";
    errEmail.innerText = "";
    errHoTen.innerText = "";
    errNhomNguoiDung.innerText = "";

    let isValid = true;

    // Kiểm tra nhóm người dùng
    if (inputMaNhom.value === "") {
        errNhomNguoiDung.innerText = "Nhóm người dùng bắt buộc!"
        isValid = false;
    }

    // Kiểm tra khoảng trắng ở giữa tên đăng nhập
    if (inputTenDangNhap.value.trim().includes(" ")) {
        errTenDangNhap.innerText = "Tên đăng nhập không chứa khoảng trắng!";
        isValid = false;
    }

    // Kiểm tra tên đăng nhập
    if (inputTenDangNhap.value.trim() === "") {
        errTenDangNhap.innerText = "Tên đăng nhập bắt buộc!";
        isValid = false;
    }

    // Kiểm tra mật khẩu
    if (inputMatKhau.value.trim() === "") {
        errMatKhau.innerText = "Mật khẩu bắt buộc!";
        isValid = false;
    }

    // Kiểm tra email
    if (inputEmail.value.trim() === "") {
        errEmail.innerText = "Email bắt buộc!";
        isValid = false;
    }

    // Kiểm tra họ tên
    if (inputHoTen.value.trim() === "") {
        errHoTen.innerText = "Họ tên bắt buộc!";
        isValid = false;
    }

    // Nếu tất cả các kiểm tra đều hợp lệ, cho phép submit form
    if (!isValid) {
        event.preventDefault();  // Ngăn submit nếu có lỗi
    }
});
