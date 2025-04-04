function calculatePackageSize() {
    var length = parseFloat(document.getElementById('Length').value) || 0;
    var width = parseFloat(document.getElementById('Width').value) || 0;
    var hight = parseFloat(document.getElementById('Hight').value) || 0;

    var packageSize = length * width * hight;

    document.getElementById('PackageSize').value = packageSize.toFixed(2);
}