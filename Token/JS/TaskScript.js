
var data = JsonConvert.SerializeObject(product);

aa();

function aa() {
    var obj = [];
    obj = { Id = 1, Name = "安慕希", Count = 10, Price = 58.8 };

    var last = JSON.stringify(obj); //将JSON对象转化为JSON字符  

    JSON.stringify(obj);
    $.post("Token/TestCacheToken", { str: last }, function (data) {
        if (data) {

        }
    });
}