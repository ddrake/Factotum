-- Initial Data for Factotum Database V. 7 

-- GLOBALS
INSERT INTO Globals(DatabaseVersion, CompatibleDBVersion, SiteActivationKey, MasterRegCheckedOn, UnverifiedSessionCount, IsMasterDB, IsNewDB, IsInactivatedDB)
values(7,4,NULL,NULL,0,0,1,0);

-- GRID SIZES
INSERT INTO GridSizes(GszDBid, GszName, GszAxialDistance, GszRadialDistance, GszMaxDiameter, GszIsLclChg, GszUsedInOutage, GszIsActive)
VALUES ('08c7fe17-5900-4afc-8bab-7b220afd2624','0.5 x 0.5','0.500','0.500','1.999',0,0,1);

INSERT INTO GridSizes(GszDBid, GszName, GszAxialDistance, GszRadialDistance, GszMaxDiameter, GszIsLclChg, GszUsedInOutage, GszIsActive)
VALUES ('7d3a35b2-dbdb-4cbf-b30f-6327323a4540','1.0 x 1.0','1.000','1.000','6.000',0,0,1);

INSERT INTO GridSizes(GszDBid, GszName, GszAxialDistance, GszRadialDistance, GszMaxDiameter, GszIsLclChg, GszUsedInOutage, GszIsActive)
VALUES ('d8942ca2-d490-4d5c-b1bd-c345137d5f58','2.0 x 2.0','2.000','2.000','10.000',0,0,1);

INSERT INTO GridSizes(GszDBid, GszName, GszAxialDistance, GszRadialDistance, GszMaxDiameter, GszIsLclChg, GszUsedInOutage, GszIsActive)
VALUES ('8834b0d8-181c-44e9-a26d-16eb39036894','3.0 x 3.0','3.000','3.000','16.000',0,0,1);

INSERT INTO GridSizes(GszDBid, GszName, GszAxialDistance, GszRadialDistance, GszMaxDiameter, GszIsLclChg, GszUsedInOutage, GszIsActive)
VALUES ('61bc940c-c09a-4d6d-b96b-f52cfb0fb5f0','4.0 x 4.0','4.000','4.000','999.999',0,0,1);


-- PIPE SCHEDULE LOOKUP
INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('edb2663d-9565-46c8-9b56-a9f066893291','0.405','0.035','Sch. 5',0.125,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('2d061400-f89a-47d9-91df-ec351a7878aa','0.540','0.049','Sch. 5',0.250,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('7a699625-47e6-48c4-b54f-a1f137ec9f75','0.675','0.049','Sch. 5',0.375,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('3d0d175b-4b05-4b16-9e78-a0e935e435f9','12.750','0.165','Sch. 5',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('5fbd5566-89cb-47f6-9473-ef45f89fa3e2','14.000','0.250','Sch. 1',14.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('f5454306-5045-48f9-92d1-9136c746d98e','16.000','0.250','Sch. 1',16.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('3619931d-df60-481a-b582-b6ac985c33a5','18.000','0.250','Sch. 1',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('15610804-b3e5-438f-b669-7d2ecf9e96db','20.000','0.250','Sch. 1',20.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('8d51d218-cacc-4f1c-ae32-b3fbb83d0261','26.000','0.312','Sch. 1',26.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('e4dae6bb-ee8e-44c0-8551-bdfc56154ac4','28.000','0.312','Sch. 1',28.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('0d0cc365-a153-417b-81d4-50be65aa2ae1','32.000','0.312','Sch. 1',32.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('1e7e5fd8-78d5-48b7-9285-e21fa959ca93','34.000','0.312','Sch. 1',34.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('60f876c8-3903-4ba5-ba96-af67fac75fab','36.000','0.312','Sch. 1',36.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('09f52b00-5888-42b6-afbc-7aa3012d9056','8.625','0.250','Sch. 2',8.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('6340682f-3ebb-4a94-92b3-4e0b5d2cd059','10.750','0.250','Sch. 2',10.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('8165c1a3-6f0b-4e62-a8a9-6cac82edc38e','12.750','0.250','Sch. 2',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('20cd2dee-2c08-4326-a4e9-916ae107b694','14.000','0.312','Sch. 2',14.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('4b242c85-01ef-4d7b-817f-ef982f149000','16.000','0.312','Sch. 2',16.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('88af5ccc-7de9-4728-8ccd-d58e07619467','18.000','0.312','Sch. 2',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('a56d5945-cfb6-4243-882f-cf9dec5066ad','28.000','0.500','Sch. 2',28.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('430ee4db-984a-457c-a5c7-c710c1dc43e4','34.000','0.500','Sch. 2',34.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('3c2c8a9a-6e0e-4d99-b97a-72ab6e435862','8.625','0.277','Sch. 3',8.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('70705b71-4126-48d6-b9aa-a35adfd15084','10.750','0.307','Sch. 3',10.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('0e916c86-9fe1-46b9-9d25-20986b46f5e4','12.750','0.330','Sch. 3',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('c9b5c77e-cca0-4ed0-8abb-e6086c12bff6','18.000','0.437','Sch. 3',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('33737ab1-4fc2-452c-a601-ce99c357fad2','24.000','0.562','Sch. 3',24.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('7e8072d9-d4b9-4520-8b33-cdb20cc0749e','28.000','0.625','Sch. 3',28.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('73aa4140-4096-4e3e-a3f2-a6019c32ae32','30.000','0.625','Sch. 3',30.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('6c2d6f67-be0e-4c0b-9a53-e9c04cdb5b4d','32.000','0.625','Sch. 3',32.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('1c0555df-5a33-4bff-9568-94ca41535e5d','34.000','0.625','Sch. 3',34.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('af61c74a-c9aa-4ab6-8b46-926c8c123662','36.000','0.625','Sch. 3',36.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('ede82393-ec00-4f48-9e34-dbd6f6d35a9b','12.750','0.406','Sch. 4',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('9944485c-75da-469b-b6ea-adfbd83bff9e','14.000','0.437','Sch. 4',14.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d5344758-12a1-4fde-8288-cf3776e3f2af','18.000','0.562','Sch. 4',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('2970dc72-4393-4500-9561-8392fca68e6d','20.000','0.593','Sch. 4',20.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d076d810-ea20-466b-ad59-eb03bc407401','24.000','0.687','Sch. 4',24.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('9ab0ee22-a2fa-49f8-9dd0-1ba7aa00b25f','32.000','0.688','Sch. 4',32.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('016f7bf0-255f-4bea-bab7-de82b73fdf0c','34.000','0.688','Sch. 4',34.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('e2e6fa3d-aa62-4f09-96e7-49e33896e79e','36.000','0.750','Sch. 4',36.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('b9e38c2f-5331-48a8-b5ab-a905524f1012','4.500','0.281','Sch. 6',4.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('9e01d9bd-e94e-4c35-b00c-d424646ededa','8.625','0.406','Sch. 6',8.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('4a5dc252-62dc-4d65-92e4-4cab97525848','12.750','0.562','Sch. 6',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('90c858af-74a8-495c-ab1c-ac39413c4b5d','14.000','0.593','Sch. 6',14.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('051391d7-e554-4559-8676-83d3fe31d509','16.000','0.656','Sch. 6',16.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('75cd39f8-f69c-4b6c-ae62-6c0297ed49c2','18.000','0.750','Sch. 6',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d20760fd-b88e-411c-a174-28b986c20b76','20.000','0.812','Sch. 6',20.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('58849c1b-ad21-4eba-8045-87e6c819e27d','24.000','0.968','Sch. 6',24.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('85e48aa3-a49f-4fce-891d-c028b266c7f4','10.750','0.593','Sch. 8',10.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('6707fc9b-1b8d-4e13-9b85-a69b77f406ae','12.750','0.687','Sch. 8',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('7921ecbd-9286-4093-bf75-7d38e7fc6101','14.000','0.750','Sch. 8',14.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('b12ad66e-9c8d-46d9-8cf9-30b8a473ff4c','16.000','0.843','Sch. 8',16.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('5dec66e7-670b-4c35-8411-fcaa37038f3c','18.000','0.937','Sch. 8',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('fcf4cb41-88b1-4358-bd22-89cae9d91ae4','20.000','1.031','Sch. 8',20.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d790fce8-5d0f-4365-a1f1-779fe53c3cc3','24.000','1.218','Sch. 8',24.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('e9f4c057-1f76-437b-be37-5d63fe147c69','8.625','0.593','Sch. 10',8.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('e0e96ef8-1153-406f-94bf-b6bef4fc45be','10.750','0.718','Sch. 10',10.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('71f012f9-1e9f-4ce0-b417-d833799d224a','12.750','0.843','Sch. 10',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('ce675c30-da30-4989-8b29-b9b720f3597d','14.000','0.937','Sch. 10',14.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('b52e09a0-5aca-4de1-92c2-5c1d84383c85','16.000','1.031','Sch. 10',16.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('933394c4-18e5-4aad-82b6-157e74a9ac7d','18.000','1.156','Sch. 10',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('039bb5ec-6240-4386-9b1f-2e56d1c8d203','20.000','1.280','Sch. 10',20.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('3edd2e17-57c6-4213-9378-850f178cbadd','24.000','1.531','Sch. 10',24.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('b7b8867e-7776-40dd-b14a-da5f591f9302','4.500','0.437','Sch. 10',4.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('e214d686-aa83-4d7c-a66f-60cb71d108a8','5.563','0.500','Sch. 10',5.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('b8909baa-de34-4568-a6a8-c8563f5564c6','6.625','0.562','Sch. 10',6.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('b6fae0d2-bb84-481c-8623-5ea08a263c8b','8.625','0.718','Sch. 10',8.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('918ae576-4f8c-4f9f-bc62-c7d61bfc3938','10.750','0.843','Sch. 10',10.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('0b4115d1-5620-4cb8-93de-dd02c0b45029','12.750','1.000','Sch. 10',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('c79fc5cf-193a-4c37-b7e5-37e3aae63229','14.000','1.093','Sch. 10',14.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('06422028-0e0e-4a4f-abd8-5c7478a9b72f','16.000','1.218','Sch. 10',16.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('1529ef1b-1068-4249-aca5-25ed1dbb2bc5','18.000','1.375','Sch. 10',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('090b9733-9030-4079-8321-9323db81334d','20.000','1.500','Sch. 10',20.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('ab31aac4-1d8d-48a1-925c-0c5187e3c2e0','24.000','1.812','Sch. 10',24.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('b0327a71-6415-47c4-8222-999a0ff54ee7','8.625','0.812','Sch. 10',8.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('6f653385-da82-4d9e-8451-12097f306db9','10.750','1.000','Sch. 10',10.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('0e3692b0-f6b4-4501-9f7f-81ff59c846a3','12.750','1.125','Sch. 10',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('8761289c-56ef-412d-a543-17867c4e4651','14.000','1.250','Sch. 10',14.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('be2cdbe1-caea-4844-b522-998633d78854','16.000','1.437','Sch. 10',16.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('edd34ad7-88bc-4898-bf42-23ab2102d25e','18.000','1.562','Sch. 10',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('95278337-ae02-4f21-9381-99e63b43ef6b','20.000','1.750','Sch. 10',20.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('0bda2730-1b3c-4244-9edf-88a05aabd272','24.000','2.062','Sch. 10',24.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d4a13831-6dfb-454d-b0cd-55a86836052b','0.840','0.187','Sch. 10',0.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('cdf639c0-8e9f-466e-8cb7-ccd9a11145ef','1.050','0.218','Sch. 10',0.750,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('43bf2dc0-a88c-4bc0-acb3-83190cae93e7','1.315','0.250','Sch. 10',1.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('f0d339ae-42b5-4f87-b367-b007822f30bb','1.660','0.250','Sch. 10',1.250,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('c201d4f8-94ab-4aeb-a26a-210268c849c0','1.900','0.281','Sch. 10',1.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('a9e96790-82d4-4f2e-98ba-12ee74c4adfc','2.375','0.343','Sch. 10',2.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('e593e791-951c-446f-a747-ffe9b7bcd9e4','2.875','0.375','Sch. 10',2.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('6a4725cc-ff57-47eb-a3fc-e0e1ba80521c','3.500','0.437','Sch. 10',3.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('0adea9c0-7b5e-4248-8c36-77a1f75a09cc','4.500','0.531','Sch. 10',4.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('231c7fe1-27ab-46b0-956d-fd2fad35efd0','5.563','0.625','Sch. 10',5.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('e250252e-1d5b-4b2d-abf6-2d7270d4d90e','6.625','0.718','Sch. 10',6.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('dadd6bd2-5fe4-4cc2-98f3-9b23faa8a498','8.625','0.906','Sch. 10',8.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('9fa44341-81b8-425a-8f04-9eef6f89f96c','10.750','1.125','Sch. 10',10.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('53ffed64-b713-4054-bb3b-587b724c256b','12.750','1.312','Sch. 10',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('36edcfd6-df27-483b-939c-f9fe0fe05dd8','14.000','1.406','Sch. 10',14.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('90042cd7-b5eb-42ce-b78e-1af2e39c3e0f','16.000','1.593','Sch. 10',16.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('bfa74a1e-16d8-4ebf-a0c0-ebc4c619010f','18.000','1.781','Sch. 10',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('975af029-59e4-4c2e-9857-50a094327b71','20.000','1.968','Sch. 10',20.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('649e2431-a5fc-4c4d-afef-da21ca9b4aa4','24.000','2.343','Sch. 10',24.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('29697302-94b4-4ce2-be00-01efc3a428d5','0.405','0.049','Sch. 10/1s',0.125,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('e2c34726-3ec0-476d-842e-3c42fb532736','0.540','0.065','Sch. 10/1s',0.250,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('94f41347-7f55-4713-aef4-9d0259505310','0.675','0.065','Sch. 10/1s',0.375,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('522e0e87-040d-4dc5-809e-8cee22d35f31','0.840','0.083','Sch. 10/1s',0.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('dda06e13-628e-4a37-97c9-6628bf719ddf','1.050','0.083','Sch. 10/1s',0.750,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('511f86a3-77d8-41d4-b527-b23da06d13d1','1.315','0.109','Sch. 10/1s',1.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('68640fb1-706b-4050-8bec-3be10bb63137','1.660','0.109','Sch. 10/1s',1.250,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('7929cfbc-1223-4c10-830a-a788d0015799','1.900','0.109','Sch. 10/1s',1.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('7dac666d-7e53-48db-a234-34985b2f332c','2.375','0.109','Sch. 10/1s',2.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d8cc3ffb-5e8f-432e-9c8b-060530e7198b','2.875','0.120','Sch. 10/1s',2.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('1438ede6-50a9-4d9e-b0d3-27d5e27ae5ed','3.500','0.120','Sch. 10/1s',3.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('80b35f52-37ec-4306-8f47-5a8c3a001d9e','4.000','0.120','Sch. 10/1s',3.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('c5a4dbb2-bc13-4414-9ee8-9bb25ec66f49','4.500','0.120','Sch. 10/1s',4.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('42953467-eb72-42f2-b8fa-7d2e147dbb0a','5.563','0.134','Sch. 10/1s',5.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('02d491fe-3cac-4809-b452-2a373b7fcfc3','6.625','0.134','Sch. 10/1s',6.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d0ac449d-7b47-459e-93ab-88646b620ce5','8.625','0.148','Sch. 10/1s',8.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('16f4f876-c16a-4dd3-bdc7-0b5635bc8b99','10.750','0.165','Sch. 10/1s',10.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d3c54881-f9dd-4fe2-afdc-c08f13b556e3','12.750','0.180','Sch. 10/1s',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d46597cf-1893-40af-9d77-c24379e4d099','14.000','0.188','Sch. 10s',14.000,0;

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('6400924a-3f8d-444e-a86c-7f015c5ea8c4','16.000','0.188','Sch. 1s',16.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('5cdd544b-32ea-45c3-9a9a-450f104fe3e2','18.000','0.188','Sch. 1s',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('ff6d3a83-44df-43ae-80ee-d1478113d56e','20.000','0.218','Sch. 1s',20.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('8ae6a35f-bf9a-46ad-b8bc-bec242605fe0','24.000','0.250','Sch. 10/1s',24.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('f4ebb760-e7f3-49d7-8565-342fada02adc','30.000','0.312','Sch. 10/1s',30.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('de425fc9-fe98-4172-905c-3de22f45f53e','0.405','0.068','Sch. 40/40s/S',d0.125,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('543507db-6dab-4775-aa48-257cef7e82f4','0.540','0.088','Sch. 40/40s/S',d0.250,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('26a46f33-e07f-4087-9231-4f6a589ecb61','0.675','0.091','Sch. 40/40s/S',d0.375,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('78f8b632-28bd-4991-83a8-40b869bb00bf','0.840','0.109','Sch. 40/40s/S',d0.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('3d4a138c-3764-4370-9624-996b92a63db7','1.050','0.113','Sch. 40/40s/S',d0.750,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('5d9bc29b-b65e-4804-8f13-e8f42faab2ed','1.315','0.133','Sch. 40/40s/S',d1.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d8654a93-9187-45d6-823c-72c67d815e2e','1.660','0.140','Sch. 40/40s/S',d1.250,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('a104e9d7-c307-4480-aa67-f6637c64208a','1.900','0.145','Sch. 40/40s/S',d1.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('cda5a49f-5930-4a23-8991-bf74e9ed2470','2.375','0.154','Sch. 40/40s/S',d2.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('ba149d8e-d6bd-4ef8-bde1-d5c99386cb54','2.875','0.203','Sch. 40/40s/S',d2.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d2aed472-a7d7-46b1-98ac-2cd3fdb57aa5','3.500','0.216','Sch. 40/40s/S',d3.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('02e33408-cc8c-454c-8bc0-4a1ae6e2c9b9','4.000','0.226','Sch. 40/40s/S',d3.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('ee947e48-db0b-48fa-9dc5-e11fbb208e78','4.500','0.237','Sch. 40/40s/S',d4.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('c56ef804-5e67-4da6-8137-52b41c33daaa','5.000','0.247','Sch. 40s/S',d4.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('4e520d98-bd59-4b85-929b-9155385915b3','5.563','0.258','Sch. 40/40s/S',d5.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('4f205385-baf7-4a6f-b88c-338474ae10ee','6.625','0.280','Sch. 40/40s/S',d6.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('1d3f1a23-2b69-440e-9c24-bc9b1f3dda91','7.625','0.301','Sch. 40s/S',d7.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('69349db0-6202-497a-878f-6c7656453280','8.625','0.322','Sch. 40/40s/S',d8.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('35c5bb23-4ab5-4afa-8e75-21d2fff47e91','9.625','0.342','Sch. 40s/S',d9.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('b9dfc67d-f46e-424c-b790-0e118232dc02','10.750','0.365','Sch. 40/40s/S',d10.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('9c816ff5-2369-4738-a719-d07b5cc040d0','11.750','0.375','Sch. 40s/S',d11.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('3a0afb01-a9a6-43db-954f-f71b6c7264c8','12.750','0.375','Sch. 40s/S',d12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('7fcbdcec-786e-4e73-ac43-46f80246e5f1','14.000','0.375','Sch. 30/40s/S',d14.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('79ab5986-07c3-4e97-a80e-9d296f6d79ba','16.000','0.375','Sch. 30/40s/S',d16.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('07a54cb4-5ee8-4cef-9b41-226009349442','18.000','0.375','Sch. 40s/S',d18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('b5943529-7561-43ec-9046-da0dd13bee81','20.000','0.375','Sch. 20/40s/S',d20.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('701803e8-f2b1-425f-a6f6-f45c4da55346','24.000','0.375','Sch. 20/40s/S',d24.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('14a56cda-632b-4354-9d12-8d3033daf714','26.000','0.375','Sch. 40s/S',d26.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('b90bbb0f-0dbf-4c33-b636-a5c727422c23','28.000','0.375','Sch. 40s/S',d28.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('1156055a-8bdc-405f-85fc-bcf7490a8ce4','30.000','0.375','Sch. 40s/S',d30.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('b36774db-fba3-497c-9c6a-57ef1d71b203','32.000','0.375','Sch. 40s/S',d32.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('0aaeb274-b58f-438e-ab8c-2286d15eb9c2','34.000','0.375','Sch. 40s/S',d34.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('f769bfe5-cdab-4474-9f21-03e5930f4755','36.000','0.375','Sch. 40s/S',d36.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('0347fc14-a110-465c-a784-1e9fff0ab464','0.840','0.065','Sch. 5/5',0.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('82497772-b6e6-433e-98ce-02eff5c3a4b2','1.050','0.065','Sch. 5/5',0.750,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('9ddf9d89-7f9a-42f1-b8b2-992dbbc96953','1.315','0.065','Sch. 5/5',1.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('a3638336-5d33-4add-98ca-ed2cc3783073','1.660','0.065','Sch. 5/5',1.250,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('7ee904a4-bbb9-4108-bdd2-e3802329f3b9','1.900','0.065','Sch. 5/5',1.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('0d77c06b-be87-42d0-8752-836a3be0bde8','2.375','0.065','Sch. 5/5',2.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('eaf32c89-e1ae-4e0b-b047-568349964ee2','2.875','0.083','Sch. 5/5',2.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('3e792f2e-aa9e-41b8-a112-99a0166366e0','3.500','0.083','Sch. 5/5',3.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('2158e860-82af-4f83-bccc-31caa6390a9b','4.000','0.083','Sch. 5/5',3.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('cce4ab17-df44-49a6-ac1d-762045901538','4.500','0.083','Sch. 5/5',4.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('734548c9-bab9-43d5-85c8-12f9bd65cc2c','5.563','0.109','Sch. 5/5',5.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('0ce69a88-65e5-44c1-a906-76fe097af5b4','6.625','0.109','Sch. 5/5',6.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d1b337f8-2693-470e-a648-33a572908e12','8.625','0.109','Sch. 5/5',8.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('77c9ac2b-daf3-4a0c-ba70-f41171e28caf','10.750','0.134','Sch. 5/5',10.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('7936f2fc-9665-48a1-b621-be685bb7003e','12.750','0.156','Sch. 5s',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('934f73c4-0ead-4a10-af42-1b73f91903c2','14.000','0.156','Sch. 5',14.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('5f52e168-0faf-4ca8-9128-b6e5d906fef3','16.000','0.165','Sch. 5',16.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('add69f25-f81f-4591-9b64-26c4a4d16fda','18.000','0.165','Sch. 5',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('4ae3c999-99f7-4de8-8b76-0f2f81508fc4','20.000','0.188','Sch. 5',20.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('826a3c6c-7db6-4b8d-b37a-c1f623e7bc50','24.000','0.218','Sch. 5',24.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('778a383d-cc23-4bb2-b706-104fee7dbd5a','30.000','0.250','Sch. 5',30.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('efc8b4a8-ad8b-403d-b61d-7a7ff26410ff','0.405','0.095','Sch. 80/80s/E.H.',0.125,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('75b1ca3c-b872-4b6b-985f-df9178521d96','0.540','0.119','Sch. 80/80s/E.H.',0.250,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('1fbb45d5-2ade-4220-b1ee-9d8cb70108d5','0.675','0.126','Sch. 80/80s/E.H.',0.375,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('ce6ffdd0-c999-404e-9665-315137820cdb','0.840','0.147','Sch. 80/80s/E.H.',0.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('fb1a3939-e05d-41a6-9f40-a0070c758296','1.050','0.154','Sch. 80/80s/E.H.',0.750,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('826977fa-c9e3-4748-9dd3-532577035f35','1.315','0.179','Sch. 80/80s/E.H.',1.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('6e4e976b-b725-4fc3-875f-bcec4267228c','1.660','0.191','Sch. 80/80s/E.H.',1.250,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('0376f86f-7c0a-4d87-8a4a-4bf96e46e218','1.900','0.200','Sch. 80/80s/E.H.',1.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('7a26e789-19af-4f45-bc2b-91b228479562','2.375','0.218','Sch. 80/80s/E.H.',2.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('059b5677-8f3d-4e7b-b5f1-46e17f08f3b8','2.875','0.276','Sch. 80/80s/E.H.',2.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('38e5428c-9a51-4175-ba8f-8a6e235bab44','3.500','0.300','Sch. 80/80s/E.H.',3.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('788908fd-5cdc-4162-9038-f22827096f0d','4.000','0.318','Sch. 80/80s/E.H.',3.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('dfd19c8c-28de-4c2d-bac6-30060f3f05d8','4.500','0.337','Sch. 80/80s/E.H.',4.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('4345e94f-965f-4c96-a945-c293bdd17492','5.000','0.355','Sch. 80s/E.H.',4.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('e13bbd39-a52b-4057-97d5-f1915ce18c40','5.563','0.375','Sch. 80/80s/E.H.',5.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('e7a7af4a-7ffd-4e8f-a57a-2f62b878948b','6.625','0.432','Sch. 80/80s/E.H.',6.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('05925931-8691-4c26-8567-302adb38a9d3','7.625','0.500','Sch. 80s/E.H.',7.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('a50f45f6-bfe2-432a-b562-4c97c6cb8b13','8.625','0.500','Sch. 80/80s/E.H.',8.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('f706da52-f0fb-4795-8afb-ea36e468c8f6','9.625','0.500','Sch. 80s/E.H.',9.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('e822da58-4b2f-4c54-8f52-2a99653599e7','10.750','0.500','Sch. 60/80s/E.H.',10.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('a12f07ff-84bf-406f-9f81-c66ff73eb32a','11.750','0.500','Sch. 80s/E.H.',11.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('389894a7-f645-473f-97cd-e2178e855f8f','12.750','0.500','Sch. 80s/E.H.',12.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('769e5ca4-7417-4ba3-ae6b-2b9193c5f4d8','14.000','0.500','Sch. 80s/E.H.',14.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('6b80d535-9bba-4fdc-b24f-d23e300076b7','16.000','0.500','Sch. 40/80s/E.H.',16.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('a5079648-2dc4-4c10-a50e-074a95924432','18.000','0.500','Sch. 80s/E.H.',18.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('17d82c0f-acba-42ec-bd5b-a56ecf377174','20.000','0.500','Sch. 30/80s/E.H.',20.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('6a86d0a0-5301-4825-aa4b-a34d9b1cf60c','24.000','0.500','Sch. 80s/E.H.',24.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('53e395d5-c227-4497-a82f-d9d84376034f','26.000','0.500','Sch. 20/80s/E.H.',26.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('98e9d6c7-7efc-4fcc-9454-837db9292c23','30.000','0.500','Sch. 20/80s/E.H.',30.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('dfbb2953-4f5f-466c-8585-a5d85184ffa4','32.000','0.500','Sch. 20/80s/E.H.',32.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('f6a528d4-a03c-4827-a4b6-9a9cc93503ce','36.000','0.500','Sch. 80s/E.H.',36.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('2978ce55-33ed-445a-99ef-b5b9c697d0f6','0.840','0.294','Sch. Dbl E.H.',0.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('26bdf309-c320-4b03-af59-68eb83847e0d','1.050','0.308','Sch. Dbl E.H.',0.750,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('49e26bad-1fc0-41f8-8b62-c3d7bc6e7b9b','1.315','0.358','Sch. Dbl E.H.',1.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('2a65699f-813a-4eb5-b594-0f354d3edca9','1.660','0.382','Sch. Dbl E.H.',1.250,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d11031b9-6768-4b02-ae83-79767ce76666','1.900','0.400','Sch. Dbl E.H.',1.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('4666e913-2804-4584-94dd-992cce946abf','2.375','0.436','Sch. Dbl E.H.',2.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('d7a37b05-0b51-4f65-80f2-24d34912955a','2.875','0.552','Sch. Dbl E.H.',2.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('63fe45b9-1e48-47de-9443-bd3598f2691d','3.500','0.600','Sch. Dbl E.H.',3.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('a73020c2-b494-4730-9173-9d52af888a82','4.000','0.636','Sch. Dbl E.H.',3.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('89dc63ce-8a56-4bff-aa51-dd28167d9d55','4.500','0.674','Sch. Dbl E.H.',4.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('b8cd96cd-ca56-41ff-9239-b649353278dd','5.000','0.710','Sch. Dbl E.H.',4.500,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('607c275b-10db-415d-94b8-22443f2a3744','5.563','0.750','Sch. Dbl E.H.',5.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('5f51ceee-1ab5-4169-b80b-e5c4aa1bbceb','6.625','0.864','Sch. Dbl E.H.',6.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('eed963f2-4002-4b70-bb5b-d4578f6fa171','7.625','0.875','Sch. Dbl E.H.',7.000,0);

INSERT INTO PipeScheduleLookup(PslDBid, PslOd, PslNomWall, PslSchedule, PslNomDia, PslIsLclChg)
VALUES('e4e30ce1-9f5e-44dd-a454-8618a4e94ec5','8.625','0.875','Sch. Dbl E.H.',8.000,0);


-- RADIAL LOCATIONS
INSERT INTO RadialLocations (RdlDBid, RdlName, RdlIsLclChg, RdlUsedInOutage, RdlIsActive)
VALUES('68c0ea77-07c6-4619-a2dc-ca8686690b1f','TDC',0,0,1)

-- SPECIAL CAL PARAMS
INSERT INTO SpecialCalParams(ScpDBid, ScpName, ScpUnits, ScpReportOrder, ScpUsedInOutage, ScpIsLclChg, ScpIsActive)
VALUES('b596a1d1-afd0-46a7-b626-1f8f0d5c488d','Coarse Delay','ms',0,0,0,1);

INSERT INTO SpecialCalParams(ScpDBid, ScpName, ScpUnits, ScpReportOrder, ScpUsedInOutage, ScpIsLclChg, ScpIsActive)
VALUES('6e56544d-8bb7-4d1f-8acd-cbca8027ba1d','Delay Calib.','ms',1,0,0,1);

INSERT INTO SpecialCalParams(ScpDBid, ScpName, ScpUnits, ScpReportOrder, ScpUsedInOutage, ScpIsLclChg, ScpIsActive)
VALUES('68598219-ea69-4a34-ba22-90a6db03f1f4','Range Calib.','in',2,0,0,1);

INSERT INTO SpecialCalParams(ScpDBid, ScpName, ScpUnits, ScpReportOrder, ScpUsedInOutage, ScpIsLclChg, ScpIsActive)
VALUES('20af6deb-8eab-429c-80a9-2d3226a96fc4','Inst. Freq.','mHz',3,0,0,1);

INSERT INTO SpecialCalParams(ScpDBid, ScpName, ScpUnits, ScpReportOrder, ScpUsedInOutage, ScpIsLclChg, ScpIsActive)
VALUES('b79bec2d-051c-4e06-9b56-b2e0211f1cff','Damping','dB',4,0,0,1);

INSERT INTO SpecialCalParams(ScpDBid, ScpName, ScpUnits, ScpReportOrder, ScpUsedInOutage, ScpIsLclChg, ScpIsActive)
VALUES('46da5b56-5aed-4ace-bc5a-d05dddf7c217','Reject','dB',5,0,0,1);

INSERT INTO SpecialCalParams(ScpDBid, ScpName, ScpUnits, ScpReportOrder, ScpUsedInOutage, ScpIsLclChg, ScpIsActive)
VALUES('cd21a2c7-6291-45e8-8019-bbf710a59d0d','Filter','dB',6,0,0,1);

INSERT INTO SpecialCalParams(ScpDBid, ScpName, ScpUnits, ScpReportOrder, ScpUsedInOutage, ScpIsLclChg, ScpIsActive)
VALUES('03390f56-295d-485c-b503-18980d0a5082','Angle','deg',7,0,0,1);

INSERT INTO SpecialCalParams(ScpDBid, ScpName, ScpUnits, ScpReportOrder, ScpUsedInOutage, ScpIsLclChg, ScpIsActive)
VALUES('49c9f604-1d8b-48bf-80ef-40a0fb713625','Surface','',8,0,0,1);

