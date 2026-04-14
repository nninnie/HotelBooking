# 🏨 Hotel Booking – Console App

<p align="center">
  <img src="images/hotell-mz.png" alt="Hotell M2" width="600"/>
</p>

Ett komplett bokningssystem för ett mindre hotell, byggt som en .NET Core Console‑applikation med Entity Framework Core, Autofac och Spectre.Console.  
All data lagras i en SQL‑databas som genereras via EF Core Code First.

## ✨ Funktionalitet

🛏️ **Rum**  
- Skapa, visa, uppdatera och ta bort rum  
- Enkelrum, dubbelrum och familjerum  
- Extrasängar beroende på rumstyp  
- Pris per natt, kapacitet och rumsinfo  

👤 **Gäster**  
- Registrera nya gäster  
- Uppdatera och visa gäster  
- Kontroll vid borttagning om gästen har bokningar  

📅 **Bokningar**  
- Boka rum för valfritt datumintervall  
- Kontroll av dubbelbokningar  
- Giltiga datum (ej dåtid, ej check‑out före check‑in)  
- Sökning av lediga rum baserat på datum + antal personer  
- Ändra och avboka bokningar  

🧾 **Fakturor & Betalningar**  
- Automatisk fakturagenerering vid bokning  
- Totalbelopp baserat på antal nätter  
- Registrera betalningar  
- Bokningar annulleras automatiskt om betalning saknas efter 10 dagar  

🔐 **Inloggning & Roller**  
- Login med användarnamn och lösenord  
- Roller (Admin/Employee)  
- Menyer baserat på roll  
- Möjlighet att logga ut och in igen  

## 🧱 Arkitektur

📦 **Data**  
- `HotelDBContext` – EF Core databas  
- `BuildDB` – migrationer + seed  

🧠 **Services (affärslogik)**  
- RoomService  
- GuestService  
- BookingService  
- InvoiceService  
- PaymentService  

🎛️ **Controllers (menyflöden)**  
- MainMenuController  
- LoginController  
- Undermenyer för rum, gäster, bokningar, fakturor  

🖥️ **UI‑lager**  
- RoomUI  
- GuestUI  
- BookingUI  
- InvoiceUI  
- PaymentUI  

UI‑klasserna hanterar input/output och använder Spectre.Console för tabeller, paneler och färg.

🧩 **Dependency Injection**  
- Autofac används för att injicera alla services, controllers och databaskomponenter.

## 🛠️ Installation

1. Klona projektet  
2. Kontrollera databasanslutningen i `appsettings.json`  
3. Kör migrationer:
4. Starta applikationen:

Databasen skapas och seedas automatiskt.

## 📦 NuGet‑paket

- Autofac  
- Microsoft.EntityFrameworkCore  
- Microsoft.EntityFrameworkCore.SqlServer  
- Microsoft.EntityFrameworkCore.Design  
- Microsoft.EntityFrameworkCore.Tools  
- Microsoft.Extensions.Configuration.Json  
- Spectre.Console  
- Spectre.Console.Extensions  

## 🧪 Tekniker

- .NET Core Console  
- Entity Framework Core (Code First)  
- MS SQL Server  
- Autofac (DI)  
- Spectre.Console  
- C#  
- LINQ  

## ✍️ Utvecklat av
**Ninnie – Hotel Booking Console App**

