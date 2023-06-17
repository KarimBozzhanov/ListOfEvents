function changeImage(imageInput) {
    var reader = new FileReader();
    var image = imageInput.files[0];
    var imageBlock = document.getElementById("imageBlock");
    var imageBtn = document.getElementById("imageBtn");
    imageBtn.style.display = "none";
    imageBlock.style.display = "block";
    reader.readAsDataURL(image);
    reader.onload = function (e) {
        document.querySelector("#imageSrc").src = e.target.result;
    }
}


function fileName(fileInput) {
    var file = fileInput.files;
    var fileName = document.getElementById("file_name");
    var fileBlock = document.getElementById("file_block");
    fileBlock.style.display = "block";
    for (var i = 0; i < file.length; i++) {
        fileName.innerHTML = limitStr(file[i].name, 50);
    }
}

function limitStr(str, n, symb) {
    if (!n && !symb) return str;
    symb = symb || '...';
    return str.substr(0, n - symb.length) + symb;
}