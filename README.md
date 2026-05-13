# SaigonRide - Distributed Vehicle Rental System

SaigonRide is a Tier-2 (Database-First MVC) smart mobility platform designed to manage distributed networks of public bicycles and electric scooters across Ho Chi Minh City. It features dynamic location-based pricing, fleet inventory management, and multi-gateway payment architecture for both Local Commuters and Foreign Tourists.

# Environment Setup & Execution

# Prerequisites
* **Visual Studio** (with ASP.NET Web and MVC workloads installed)
* **SQL Server Management Studio (SSMS)**

# Part 1. Database Configuration
To ensure the application runs with the correct schema and test data (stations, vehicles, and past transactions for reports), please follow these steps:
1. Open SQL Server Management Studio (SSMS) and connect to your local SQL Server instance.
2. Open the provided SQL script located at: `Database/SaigonRide_FinalProject.sql`.
3. Execute the script. This will automatically generate the database and populate it with all necessary mock data.
4. Open the `SaigonRide_FinalProject.sln` file in Visual Studio.
5. Open the `Web.config` file in the root directory and update the `connectionStrings` section to match your local SQL Server instance name (e.g., replace `Data Source=YOUR_SERVER_NAME`).

# 2. Running the Application
1. In Visual Studio, ensure the `SaigonRide_FinalProject` project is set as the Startup Project.
2. (Optional but recommended) Go to **Build > Clean Solution**, then **Build > Build Solution**.
3. Press `F5` or click **IIS Express (Run)** to build and launch the application in your default web browser.

# System Logins

Please use the following test accounts to navigate the different roles within the application. 

**Admin Dashboard Access**
* **Email:** admin@saigonride.vn
* **Password:** Admin123! 

**Local Commuter (MoMo / VNPay / Cash)**
* **Email:** nguyen@example.com
* **Password:** User123!

**Foreign Tourist (Apple Pay / PayPal / Cash)**
* **Email:** john@example.com
* **Password:** User123!
