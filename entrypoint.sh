#!/bin/bash

# Stop the script when something goes wrong.
set -e
run_cmd="dotnet watch run"

# Wait until the SQL server container is up, then run the database migration process.
until dotnet ef database update; do
>&2 echo "SQL server is starting up"
sleep 1
done

# After the migration is done, start the API app.
>&2 echo "SQL server is up - executing command"
exec $run_cmd
