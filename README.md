# Todo
ASP.NET MVC & EF Core 5 重點筆記
- WebAPI練習清單
    1. 安裝NuGet套件
    2. 建立DBFirst/CodeFirst專案
    3. 基本CRUD功能
    4. Dto，設定輸入參數或輸出參數
    5. 運用AutoMapper達成Dto與原始資料型態的互相轉換
    6. 商業邏輯寫在另外一個類別裡面，並用DI注入的方式引用
    7. 回傳符合規範的資料(IActionResult)
    8. Json字串****還原序列化的方式取得多筆Id，並刪除多筆資料****
    9. Model資料驗證
    10. 上傳檔案 & ModelBinder自訂模型繫結
- 安裝以下NuGet套件
    
    ```csharp
    Microsoft.EntityFrameworkCore.SqlServer
    Microsoft.EntityFrameworkCore.Tools
    ```
    
    ![Untitled](https://s3-us-west-2.amazonaws.com/secure.notion-static.com/816579ba-484b-442a-b4dc-d335ffa9adaf/Untitled.png)
    
- 其他參考文件
    
    
    Web API教學影片：
    
    [ASP.NET Core Web API 入門教學](https://youtube.com/playlist?list=PLneJIGUTIItsqHp_8AbKWb7gyWDZ6pQyz)
    
    EF Core 解決json循環引用問題：
    
    [https://www.cnblogs.com/liuzongxian/p/15817783.html](https://www.cnblogs.com/liuzongxian/p/15817783.html)
    
    **asp.net core 3.1 MVC/WebApi JSON 全局配置：**
    
    [https://www.cnblogs.com/51net/p/12457730.html](https://www.cnblogs.com/51net/p/12457730.html)
    
    [ASP.NET Core 簡介](https://docs.microsoft.com/zh-tw/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0#recommended-learning-path)
    
    [ASP.NET Core Web API 中的 JsonPatch](https://docs.microsoft.com/zh-tw/aspnet/core/web-api/jsonpatch?view=aspnetcore-6.0#the-add-operation)
    
    [ControllerBase 類別 (Microsoft.AspNetCore.Mvc)](https://docs.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.mvc.controllerbase?view=aspnetcore-6.0)
    
    [ASP.NET Core 中的資料繫結](https://docs.microsoft.com/zh-tw/aspnet/core/mvc/models/model-binding?view=aspnetcore-6.0#complex-types)
    

[建立CodeFirst專案](https://www.notion.so/CodeFirst-2caab6dd3fbf4027913b1dd3c509da4e)

[建立DBFirst專案](https://www.notion.so/DBFirst-95d7783b1e72462e802d77fc2f670b75)

[**DI依賴注入之生命週期**](https://www.notion.so/DI-cdab025f9eda48b19417c617ce0debe6)

[自訂模型繫結](https://www.notion.so/fef5e6862e19459e8635e042fcf18387)

[(Model**資料驗證**)**類別內自訂屬性資料驗證**](https://www.notion.so/Model-3e9cac7660fe47bfbbb7a6d3c4d9c8d7)

[(Model**資料驗證**)**自訂模型資料驗證標籤**](https://www.notion.so/Model-c3e810764d6f45a2b1761353e16fc532)

[**使用AutoMapper一行指令自動匹配DTO欄位資料**](https://www.notion.so/AutoMapper-DTO-61683518b2b74e019584976833b751cc)

[解決刪除資料時無法刪除底下子資料的問題](https://www.notion.so/55c26799e5c747fa96a12c577753257c)

[加入靜態目錄存取](https://www.notion.so/7b85a90a27f146109791059f52cf0ca9)