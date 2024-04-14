
DB_NAME="orders"
DB_USER="dean"
DB_PASSWORD="abc"
DB_CONNECTION="Host=localhost;Database=orders;Username=dean;Password=abc" 
DB_HOST="localhost"
DB_PORT="5432"

MIGRATIONS_DIR="../OMS.Infrastructure/Migrations" # e.g., ./Data/Migrations
PROJECT_DIR="../OMS.Infrastructure" # e.g., ./MyApp
PROJECT_FILE="../OMS.Infrastructure/OMS.Infrastructure.csproj" # e.g., ./MyApp
DB_CONTEXT="OrderManagementDbContext" #

EF_ASSEMBLY="OMS.Infrastructure" 
START_UP_PROJECT="Migrations.csproj"


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

    #echo "****** Processing migration files Press any key to continue ..."
    #read " "

    dotnet ef database update  -p ${PROJECT_DIR} -c $DB_CONTEXT --connection $DB_CONNECTION  --startup-project  ${START_UP_PROJECT} 
}

add_publication() {

    echo "Adding publication..."

    SQL_COMMAND="CREATE PUBLICATION liveupdates
    FOR TABLE public.\"ClosedTrades\", public.\"LiveOrders\", public.\"ScoreCards\", public.\"ModelOrderLog\"
    WITH (publish = 'insert, update, delete, truncate', publish_via_partition_root = false);"

    #CREATE PUBLICATION logupdates
    #FOR TABLE public."OrderLog", public."ScoreCardLog", public."ClosedTrades", public."ModelOrderLog"
    #WITH (publish = 'insert, update, delete, truncate', publish_via_partition_root = false);    

    # Directly pass the SQL command to psql without using eval or complex escaping
    echo "${SQL_COMMAND}" | psql -h "${DB_HOST}" -p "${DB_PORT}" -U "${DB_USER}" -d "${DB_NAME}"

    SQL_COMMAND2="CREATE PUBLICATION logupdates
    FOR TABLE public.\"ClosedTradeLog\", public.\"ModelOrderLog\", public.\"ScoreCardLog\", public.\"OrderLog\"
    WITH (publish = 'insert, update, delete, truncate', publish_via_partition_root = false);"

    echo "${SQL_COMMAND2}" | psql -h "${DB_HOST}" -p "${DB_PORT}" -U "${DB_USER}" -d "${DB_NAME}"


    # Check for errors
    if [[ $? -ne 0 ]]; then
        echo "Error creating publication liveupdates!"
        exit 1
    fi

    echo "Publication liveupdates created successfully."


}
   



# Main script execution
dropDatabase
removeMigrations
recreateDatabase
add_publication 

echo "Database reset complete."
