# casectrl_tasks
This microservice make CRUD operations on tasks
To run:
1. Open solution in Visual Studio 2022 Community Edition
2. Add users Secrets file
Below is the User Secrets file format
{
   "CustomSettings:DatabaseConnection": - connection string to any empty SQL Server Database.
   "CustomSettings:Jwt:Key": "AJjowquxjJOIH*&QS870jqw8w8h",
   "CustomSettings:RabbitMq:Connection": "amqps://mpvopxhb:onkEkCB73HZzgvnSZ4eszW4dWGTjI8lc@moose.rmq.cloudamqp.com:5671/mpvopxhb"
}
3. In package management console in Visual Studio run Update-Database to create required tables
4. Run in Debug/Release mode

Rabbit MQ is deployed on cloudamqp.com. Login: casectrl123@outlook.com Password: Aszx9080-