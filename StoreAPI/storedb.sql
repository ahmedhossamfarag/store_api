create table Product(
	id int primary key,
	name varchar(50),
	price float(53),
	img varchar(50),
	discreption varchar(200)
	)
go

create table OrderP(
	id int primary key,
	requesttime datetime,
	location varchar(50)
	)
go

create table OrderProducts(
	orderid int,
	productid int,
	count int,
	primary key(orderid, productid)
	)
go


--drop table Product
--go

--drop table OrderP
--go

--drop table OrderProducts
--go