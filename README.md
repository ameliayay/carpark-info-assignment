# Carpark-Info

## Tech Stack
- .NET 10
- SQLite + Entity Framework Core
- JWT Authentication
- CsvHelper

## How to Run
### 1. Clone the repo
- git clone https://github.com/ameliayay/carpark-info-assignment.git
- cd carpark-info-assignment/CarPark

### 2. Run the app
dotnet run

### 3. Open Swagger UI
http://localhost:5185/scalar/v1

## Testing the API
### Step 1: Import CSV data
curl -X POST http://localhost:5185/api/batch/import/hdb-carpark-information-20220824010400.csv

### Step 2: Register
curl -X POST http://localhost:5185/api/auth/register -H "Content-Type: application/json" -d "{\"username\":\"amelia\",\"email\":\"amelia@email.com\",\"password\":\"password123\"}"

### Step 3: Login and Save Token
curl -X POST http://localhost:5185/api/auth/login -H "Content-Type: application/json" -d "{\"username\":\"amelia\",\"password\":\"password123\"}"

Copy the token from the response, then save it:
set TOKEN=!!!PASTE TOKEN!!!

### Step 4: Filter Carparks (No Token Needed)
Get All Carparks
- curl http://localhost:5185/api/carparks

Filter by Free Parking
- curl "http://localhost:5185/api/carparks?freeParking=true"

Filter by Night Parking
- curl "http://localhost:5185/api/carparks?nightParking=true"

Filter by Vehicle Height
- curl "http://localhost:5185/api/carparks?minVehicleHeight=2.1"

Combine All Filters
- curl "http://localhost:5185/api/carparks?freeParking=true&nightParking=true&minVehicleHeight=2.1"

Get Single Carpark
- curl http://localhost:5185/api/carparks/ACB

### Step 5: Test favourite
Add Favourite
- curl -X POST http://localhost:5185/api/favourites -H "Content-Type: application/json" -H "Authorization: Bearer %TOKEN%" -d "{\"carParkNo\":\"ACB\"}"

Get Favourites
- curl http://localhost:5185/api/favourites -H "Authorization: Bearer %TOKEN%"

Remove Favourite
- curl -X DELETE http://localhost:5185/api/favourites/ACB -H "Authorization: Bearer %TOKEN%"

## ER Diagram
<img width="991" height="501" alt="Carpark" src="https://github.com/user-attachments/assets/9d2b20d9-4045-4ad7-a17b-5e7e2c5bbfa1" />




