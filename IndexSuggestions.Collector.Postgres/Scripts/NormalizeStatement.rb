require 'pg_query'
statement = gets.chomp
print PgQuery.normalize(statement)