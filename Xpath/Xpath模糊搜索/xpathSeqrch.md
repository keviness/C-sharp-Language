## 一，xpath的模糊定位方法：
### ①用contains关键字，如：
driver.findElement(By.xpath("//a[contains(@href,'logout')]"));
解释：寻找页面中href属性值包含有logout这个单词的所有a元素

### ②用start-with，定位代码如下：
driver.findElement(By.xpath("//a[starts-with(@href,'logout')]"));
解释：寻找href属性以logout开头的a元素，其中@后面的href可替换为其他任意属性

### ③用ends-with，定位代码如下：
driver.findElement(By.xpath("//a[ends-with(@href,'logout')]"));
解释：寻找href属性以logout结束的a元素，其中@后面的href可替换为其他任意属性

### ④用text（）关键字，定位代码如下：
driver.findElement(By.xpath("//a[contains(text(),退出)]"));
解释：寻找页面中所有包含退出的a元素

## 二，xpath定位当前元素的兄弟元素/相邻元素：
### ①前N位
../div[@="class"]/preceding-sibling::div[N]
### ②后N位：
../div[@="class"]/following-sibling::div[N]
