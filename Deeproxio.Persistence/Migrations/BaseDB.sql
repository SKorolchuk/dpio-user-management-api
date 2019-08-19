create database AIConception;

create table news
(
	id INTEGER DEFAULT nextval('news_id_seq'::regclass) PRIMARY KEY NOT NULL,
	name varchar(255),
	description varchar(8000),
	createts timestamp,
	updatets timestamp
);

create unique index news_id_uindex on news (id);

create unique index news_name_uindex on news (name);
