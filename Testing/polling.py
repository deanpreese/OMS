import psycopg2
import time
import pandas as pd

# Database connection parameters    
dbname = "orders"
user = "dean"
password = "abc"
host = "localhost"  # e.g., "localhost" or an IP address

# SQL query to execute 
open_query = 'SELECT * FROM "LiveOrder";'
closed_query = 'SELECT * FROM "ClosedTrades";'

# SQL query to execute 
open_clear = 'DELETE FROM "LiveOrder";'
closed_clear = 'DELETE FROM "ClosedTrades";'



def poll_clean_open():
    try:
        # Connect to your postgres DB
        conn = psycopg2.connect(dbname=dbname, user=user, password=password, host=host)
        # Open a cursor to perform database operations
        cur = conn.cursor()
        # Execute a query
        cur.execute(open_clear)
        conn.commit()
        print("Rows deleted successfully.")
        cur.close()
        conn.close()

    except Exception as e:
        print(f"Error: {e}")
        return None


def poll_clean_closed():
    try:
        # Connect to your postgres DB
        conn = psycopg2.connect(dbname=dbname, user=user, password=password, host=host)
        # Open a cursor to perform database operations
        cur = conn.cursor()
        # Execute a query
        cur.execute(closed_clear)
        conn.commit()
        cur.close()
        conn.close()

    except Exception as e:
        print(f"Error: {e}")
        return None




def poll_open():
    try:
        # Connect to your postgres DB
        conn = psycopg2.connect(dbname=dbname, user=user, password=password, host=host)
        # Open a cursor to perform database operations
        cur = conn.cursor()
        # Execute a query
        cur.execute(open_query)
        # Retrieve query results
        records = cur.fetchall()
        # Clean up
        cur.close()
        conn.close()
        return records

    except Exception as e:
        print(f"Error: {e}")
        return None


def poll_closed():
    try:
        # Connect to your postgres DB
        conn = psycopg2.connect(dbname=dbname, user=user, password=password, host=host)
        # Open a cursor to perform database operations
        cur = conn.cursor()
        # Execute a query
        cur.execute(closed_query)
        # Retrieve query results
        records = cur.fetchall()
        # Clean up
        cur.close()
        conn.close()
        return records

    except Exception as e:
        print(f"Error: {e}")
        return None


poll_clean_open()
poll_clean_closed()


# Polling interval in seconds
polling_interval = 1
number_of_polls = 0
poll_rows = 0

# Poll the database repeatedly
while True:
    print(f"Polling database... {number_of_polls} ")
    
    data_open = poll_open()
    df_open = pd.DataFrame(data_open)
    data_closed = poll_closed()
    df_closed = pd.DataFrame(data_closed)
       
    print(" ")
    print("OPEN ORDERS ")
    print(df_open)
    print(" ")
    print("closed orders ")
   
    if len(df_closed) > 20 :
        print(len(df_closed))
    else:
        print(df_closed)
    print(" ")
    print(" ")
    
    #if (poll_rows == len(df_closed) and len(df_closed) > 0):
    #    exit()   
    
    number_of_polls += 1   
    poll_rows = len(df_closed)     
    time.sleep(polling_interval)