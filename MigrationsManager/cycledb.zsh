
DB_NAME="orders"
DB_USER="dean"
DB_PASSWORD="abc"
MIGRATIONS_DIR="../OMS.DATA/Migrations" # e.g., ./Data/Migrations
PROJECT_DIR="../OMS.Data" # e.g., ./MyApp
PROJECT_FILE="../OMS.Data/OMS.Data.csproj" # e.g., ./MyApp
DB_CONTEXT="OrderManagementDbContext" #
DB_CONNECTION="Host=127.0.0.1;Database=orders;Username=trading;Password=abc" 
EF_ASSEMBLY="OMS.Data" 
START_UP_PROJECT="MigrationsManager.csproj"


# Function to drop the PostgreSQL database
dropDatabase() {
    echo "Dropping database $DB_NAME..."
    PGPASSWORD=$DB_PASSWORD dropdb -U $DB_USER --force $DB_NAME 
    PGPASSWORD=$DB_PASSWORD createdb -U $DB_USER $DB_NAME
}


# Function to remove all migration files
removeMigrations() {
    echo "Removing migration files in $MIGRATIONS_DIR..."
    rm -rf $MIGRATIONS_DIR/*.cs
}


# Function to recreate the database using EF Core
recreateDatabase() {

    dotnet ef migrations add InitialCreate --project ${PROJECT_DIR} -c $DB_CONTEXT --startup-project ${START_UP_PROJECT} 
    dotnet ef database update  -p ${PROJECT_DIR} -c $DB_CONTEXT --connection $DB_CONNECTION  --startup-project  ${START_UP_PROJECT} 
}


# Main script execution
dropDatabase
removeMigrations
recreateDatabase

echo "Database reset complete."
