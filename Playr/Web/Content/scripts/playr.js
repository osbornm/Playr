// Define some handy Closures
if (typeof (models) == "undefined") models = {};

if (typeof (helpers) == "undefined") helpers = {};

helpers.padNumber = function (number, width) {
    width -= number.toString().length;
    if (width > 0) {
        return new Array(width + (/\./.test(number) ? 2 : 1)).join('0') + number;
    }
    return number + "";
};

helpers.RandomNumber = function (number) {
    return Math.floor(Math.random() * number) + 1;
};

// fisher Yates Method
helpers.SortRandom = function (myArray) {
    var i = myArray.length;
    if (i == 0) return false;
    while (--i) {
        var j = Math.floor(Math.random() * (i + 1));
        var tempi = myArray[i];
        var tempj = myArray[j];
        myArray[i] = tempj;
        myArray[j] = tempi;
    }
}