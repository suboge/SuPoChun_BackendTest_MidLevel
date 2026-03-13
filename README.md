# 蘇柏鈞 (Su Po-Chun) - 後端工程師技術測試

本專案為後端工程師中階技術測試，使用 **.NET 8 Web API** 實作完整資源導向（Resource-based）之 CRUD 功能，並整合 SQL Server 預存程序進行主鍵產生。

## 🚀 快速啟動指南

### 1. 環境需求
- **Runtime**: .NET 8.0 SDK
- **Database**: SQL Server 2019+ (建議使用 Docker 執行)
- **IDE**: Visual Studio 2022 / VS Code

### 2. 資料庫還原 (Database Setup)
本專案提供 `.bak` 備份檔，請依照以下步驟還原：
1. 將根目錄下的 `Myoffice_ACPD.bak` 檔案移至 SQL Server 可存取之路徑。
2. 執行還原指令或透過 SSMS 介面進行還原，資料庫名稱請設定為 `Myoffice_ACPD`。
3. **連線字串設定**：請修改 `appsettings.json` 中的 `ConnectionStrings:DefaultConnection`，填入您的 SQL Server 伺服器位址與驗證資訊。

### 3. 執行日誌
關於執行日誌 (ExecutionLog)：
專案架構已考量到維運需求，雖然目前 CRUD 以核心邏輯為主，usp_AddLog 將 API 異常資訊記錄至 MyOffice_ExcuteionLog 表中，以利後續追蹤與排錯。

### 4. 執行專案
在專案根目錄開啟終端機，執行以下指令：
```bash
dotnet run

