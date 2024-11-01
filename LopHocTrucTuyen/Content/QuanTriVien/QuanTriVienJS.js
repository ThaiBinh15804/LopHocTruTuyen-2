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

const selectMaNhom = document.getElementById("MaNhom");
document.getElementById("btnClearForm").addEventListener("click", function(event) {
    event.preventDefault();
    inputTenDangNhap.value = "";
    inputMatKhau.value = "";
    inputEmail.value = "";
    inputHoTen.value = "";
    selectMaNhom.value = "";
})
// xử lý form thêm quản trị viên
document.getElementById("myForm1").addEventListener("submit", handleForm1);
function handleForm1() {
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
    let regexEmail = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (inputEmail.value.trim() === "") {
        errEmail.innerText = "Email bắt buộc!";
        isValid = false;
    } else if (!regexEmail.test(inputEmail.value)) {
        errEmail.innerText = "Email không hợp lệ!";
        isValid = false;
    }

    // Kiểm tra họ tên và ko chứa kí tự số
    let regexHoTen = /\d/; // \d đại diện kí tự số
    if (inputHoTen.value.trim() === "") {
        errHoTen.innerText = "Họ tên bắt buộc!";
        isValid = false;
    } else if (regexHoTen.test(inputHoTen.value)) {
        errHoTen.innerText = "Họ tên không chứa kí tự số!";
    }

    // Nếu tất cả các kiểm tra đều hợp lệ, cho phép submit form
    if (!isValid) {
        event.preventDefault();  // Ngăn submit nếu có lỗi
    }
}

document.getElementById("myForm2").addEventListener("submit", handleForm2);
//function handleForm2() {
//    const errTenDangNhap1 = document.querySelector(".errTenDangNhap1");
//    const errMatKhau1 = document.querySelector(".errMatKhau1");
//    const errEmail1 = document.querySelector(".errEmail1");
//    const errHoTen1 = document.querySelector(".errHoTen1");
//    const errSoDienThoai1 = document.querySelector(".errSoDienThoai1");
//    const errDiaChi1 = document.querySelector(".errDiaChi1");
//    const errChuyenNganh1 = document.querySelector(".errChuyenNganh1");

//    const inputTenDangNhap1 = document.getElementById("TenDangNhap1");
//    const inputMatKhau1 = document.getElementById("MatKhau1");
//    const inputEmail1 = document.getElementById("Email1");
//    const inputHoTen1 = document.getElementById("HoTen1");
//    const inputSoDienThoai1 = document.getElementById("SoDienThoai1");
//    const inputDiaChi1 = document.getElementById("DiaChi1");
//    const inputChuyenNganh1 = document.getElementById("ChuyenNganh1");

//    // Reset lỗi trước khi validate
//    errTenDangNhap1.innerText = "";
//    errMatKhau1.innerText = "";
//    errEmail1.innerText = "";
//    errHoTen1.innerText = "";
//    errSoDienThoai1.innerText = "";
//    errDiaChi1.innerText = "";
//    errChuyenNganh1.innerText = "";

//    let isValid = true;

//    // Kiểm tra khoảng trắng ở giữa tên đăng nhập
//    if (inputTenDangNhap1.value.trim().includes(" ")) {
//        errTenDangNhap1.innerText = "Tên đăng nhập không chứa khoảng trắng!";
//        isValid = false;
//    }

//    // Kiểm tra tên đăng nhập
//    if (inputTenDangNhap1.value.trim() === "") {
//        errTenDangNhap1.innerText = "Tên đăng nhập bắt buộc!";
//        isValid = false;
//    }

//    // Kiểm tra mật khẩu
//    if (inputMatKhau1.value.trim() === "") {
//        errMatKhau1.innerText = "Mật khẩu bắt buộc!";
//        isValid = false;
//    }

//    // Kiểm tra email
//    let regexEmail = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
//    if (inputEmail1.value.trim() === "") {
//        errEmail1.innerText = "Email bắt buộc!";
//        isValid = false;
//    } else if (!regexEmail.test(inputEmail1.value)) {
//        errEmail1.innerText = "Email không hợp lệ!";
//        isValid = false;
//    }

//    // Kiểm tra họ tên và ko chứa kí tự số
//    let regexHoTen = /\d/; // \d đại diện kí tự số
//    if (inputHoTen1.value.trim() === "") {
//        errHoTen1.innerText = "Họ tên bắt buộc!";
//        isValid = false;
//    } else if (regexHoTen.test(inputHoTen1.value)) {
//        errHoTen1.innerText = "Họ tên không chứa kí tự số!";
//    }

//    // Nếu tất cả các kiểm tra đều hợp lệ, cho phép submit form
//    if (!isValid) {
//        event.preventDefault();  // Ngăn submit nếu có lỗi
//    }
//}