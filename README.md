# 🏨 Hotel Booking – Console App

<p align="center">
  <img src="hotell-mz.png" alt="Hotell M2" width="600"/>
</p>

Ett komplett bokningssystem för ett mindre hotell, byggt som en .NET Core Console‑applikation med Entity Framework Core, Autofac och Spectre.Console.  
All data lagras i en SQL‑databas som genereras via EF Core Code First.

## ✨ Funktionalitet

🛏️ **Rum**  
- Skapa, visa, uppdatera och ta bort rum  
- Enkelrum, dubbelrum, familjerum  och svit
- Extrasängar beroende på rumstyp (kvm)  
- Pris per natt, kapacitet och rumsinfo  

👤 **Gäster**  
- Skapa, visa, uppdatera och ta bort gäster  
- Kontroll vid borttagning om gästen har bokningar  

📅 **Bokningar**  
- Skapa, visa, uppdatera och ta bort bokning  
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

## 📦 Data
Data‑lagret innehåller allt som rör databasen och kopplingen mellan C# och SQL.

- **HotelDBContext** – EF Core‑databasen och alla DbSets  
- **BuildDB** – ansvarar för migreringar och seed‑data  


## 🧠 Services (affärslogik)
Services‑lagret innehåller all affärslogik.  
Här ligger reglerna för hur gäster, rum, bokningar, fakturor och betalningar ska hanteras.

- **RoomService** – logik för rum  
- **GuestService** – logik för gäster  
- **BookingService** – logik för bokningar och datumkontroller  
- **InvoiceService** – logik för fakturor  
- **PaymentService** – logik för betalstatus  

**Notering:**  
Jag använder **inga repositories**.  
Services pratar direkt med **HotelDBContext**, vilket är ett renare och mer modernt sätt att arbeta med EF Core eftersom EF Core redan fungerar som repository + unit‑of‑work, så ett extra repository‑lager hade bara skapat onödig och duplicerad kod.
- DbSet fungerar som repository
- DbContext fungerar som unit‑of‑work
- 
## 🎛️ Controllers (menyflöden)
Controllers styr navigeringen i systemet och kopplar ihop UI med services.

- **MainMenuController** – huvudmenyn och navigation  
- **LoginController** – inloggning och åtkomst  
- **RoomController** – undermeny för rum  
- **GuestController** – undermeny för gäster  
- **BookingController** – undermeny för bokningar  
- **InvoiceController** – undermeny för fakturor  
- **PaymentController** – undermeny för betalningar  

Controllers ansvarar för flödet, medan UI‑klasserna visar innehållet och Services utför logiken.


## 🖥️ UI‑lager
UI‑lagret innehåller alla användarflöden och menyer. Varje UI‑klass ansvarar för ett specifikt område i systemet, vilket gör strukturen tydlig, separerad och lätt att underhålla.

- **MainMenuUI** – huvudmenyn och navigering  
- **CrudMenuUI** – CRUD‑menyer för gäster, rum och bokningar  
- **BookingUI** – bokningsflöden, datumval och logik kopplat till bokningar  
- **GuestUI** – hanterar alla flöden för gäster  
- **RoomUI** – hanterar alla flöden för rum  
- **InvoiceUI** – visar och hanterar fakturor  
- **PaymentUI** – hanterar betalstatus och betalningsflöden  
- **BookingPrompts** – all input och validering för bokningar  
- **BookingRenderer** – tabeller, paneler och visuell presentation av bokningar  
- **CalenderPicker** – kalendern för datumval  
- **BannerUI** – visar projektets banner och branding  
- **SpectreCenter** – centrerings‑hjälp för UI‑element  
- **EscUI** – hanterar ESC‑flöden och återgångar i menyer  

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

## 📄 Dokument

- ER‑diagram
- 3NF‑dokument
- SQL‑frågor 

### 🧩 Arkitektur, SOLID & lärdomar
Jag har arbetat enligt **SOLID‑principerna**, framför allt **Single Responsibility Principle**, där varje klass har ett tydligt och avgränsat ansvar.  
Exempel: 
*BookingPrompts* (input) 
*BookingRenderer* (visning)
*CalendarPicker* (datumval)
*BannerUI* (header)
*EscUI* (ESC‑flöden)
*Services* (affärslogik)
*Controllers* (menyflöden) 
*DbContext* (databas)
Detta gav ett mer strukturerat, testbart och lättutbyggt system.

Jag följde även **DIP (Dependency Inversion Principle)** genom att använda **Autofac** för beroendeinjektion.  
Det gjorde koden mer flexibel, minskade kopplingar mellan klasser och gjorde det enkelt att byta implementationer utan att ändra i UI eller controllers.

**Spectre.Console** hjälpte mig att skapa ett tydligt och visuellt UI med paneler, tabeller och färger.  
Jag valde också att **seeda bokningar** för att enklare kunna testa datumlogik och lediga rum.

Jag använder inga repositories eftersom EF Core redan fungerar som både repository (DbSet) och unit‑of‑work (DbContext).  
Ett extra repository‑lager hade bara skapat onödig och duplicerad kod, så mina services arbetar direkt mot DbContext enligt modern EF Core‑praxis. (Unit‑of‑Work betyder att alla databasändringar hanteras som en enda transaktion, så antingen sparas allt eller så rullas allt tillbaka om något går fel.)

En viktig lärdom är att jag hade velat lägga mer tid på **planeringen av mina POCO‑modeller**.  
Jag arbetade iterativt och uppdaterade modellerna flera gånger, och till slut valde jag att rensa databasen och migrationerna för att bygga upp allt från början igen.  
Det gjorde mig betydligt tryggare i hur modeller, relationer och EF Core hänger ihop och gav mig en djupare förståelse för datamodellering.


## ✍️ Utvecklat av
**Ninnie – Hotel Booking Console App**

