
USE [SoheilDb]

--INSERT INTO AccessRules ([Id],[Code],[Name],[Parent_Id]) VALUES
--(1,'0','txtSoheil',null),
--	(2,'1','txtUsers',1),
--		(3,'11','txtUserAccounts',2),
--			(4,'111','txtUsers',3),
--			(5,'112','txtPositions',3),
--			(6,'113','txtOrgCharts',3),
--		(7,'12','txtModules',2),
--			(8,'121','txtModules',7),
--		(41,'13','txtOrganizationCalendar',2),
--			(42,'131','txtWorkProfiles',41),
--			(43,'132','txtHolidays',41),
--			(44,'133','txtWorkProfilePlan',41),
--	(9,'2','txtDefinitions',1),
--		(10,'21','txtProducts',9),
--			(11,'211','txtProducts',10),
--			(12,'212','txtReworks',10),
--		(13,'22','txtDiagnosis',9),
--			(14,'221','txtDefections',13),
--			(15,'222','txtRoots',13),
--			(16,'223','txtActionPlans',13),
--			(17,'224','txtCauses',13),
--		(18,'23','txtFPC',9),
--			(19,'231','txtFPC',18),
--			(20,'232','txtStations',18),
--			(21,'233','txtMachines',18),
--			(22,'234','txtActivities',18),
--		(23,'24','txtOperators',9),
--			(24,'241','txtOperators',23),
--			(25,'242','txtGenSkills',23),
--			(26,'243','txtSpeSkills',23),
--		(27,'25','txtCosts',9),
--			(28,'251','txtCosts',27),
--			(29,'252','txtPartWarehouses',27),
--		(45,'26','txtSetupTimes',9),
--		(46,'27','txtSkillCenter',9),
--	(30,'3','txtControl',1),
--		(31,'31','txtProductPlan',30),
--		(32,'32','txtPerformance',30),
--		(33,'33','txtIndices',30),
--	(34,'4','txtReports',1),
--		(35,'41','txtCostReports',34),
--		(36,'42','txtActualCostReports',34),
--		(37,'43','txtOperationReports',34),
--	(38,'5','txtOptions',1),
--		(39,'51','txtSettings',37),
--		(40,'52','txtHelp',37),
--		(47,'53','txtAbout',37),
--	(48,'6','txtStorage',1),
--		(49,'61','txtWarehouse',48),
--		(50,'62','txtWarehouseTransactions',48),
--		(51,'63','txtRawMaterials',48),
--		(52,'64','txtUnits',52),
--			(53,'641','txtUnitSets',52),
--			(54,'642','txtUnitConversions',52);

--ALTER TABLE USERS ADD CONSTRAINT USER_UNIQUE_CODE UNIQUE (CODE);


INSERT INTO Users(Code,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,Username,[Password],Title,[Status]) VALUES
(0,0,'2013-01-01',0,'2013-01-01','Admin','fromdust','Admin',1);

INSERT INTO User_AccessRules(AccessRule_Id, [User_Id], ModifiedBy,ModifiedDate,[Type]) VALUES
(1,1,0,'2013-01-01',31),
(2,1,0,'2013-01-01',31),
(3,1,0,'2013-01-01',31),
(4,1,0,'2013-01-01',31),
(5,1,0,'2013-01-01',31),
(6,1,0,'2013-01-01',31),
(7,1,0,'2013-01-01',31),
(8,1,0,'2013-01-01',31),
(9,1,0,'2013-01-01',31),
(10,1,0,'2013-01-01',31),
(11,1,0,'2013-01-01',31),
(12,1,0,'2013-01-01',31),
(13,1,0,'2013-01-01',31),
(14,1,0,'2013-01-01',31),
(15,1,0,'2013-01-01',31),
(16,1,0,'2013-01-01',31),
(17,1,0,'2013-01-01',31),
(18,1,0,'2013-01-01',31),
(19,1,0,'2013-01-01',31),
(20,1,0,'2013-01-01',31),
(21,1,0,'2013-01-01',31),
(22,1,0,'2013-01-01',31),
(23,1,0,'2013-01-01',31),
(24,1,0,'2013-01-01',31),
(25,1,0,'2013-01-01',31),
(26,1,0,'2013-01-01',31),
(27,1,0,'2013-01-01',31),
(28,1,0,'2013-01-01',31),
(29,1,0,'2013-01-01',31),
(30,1,0,'2013-01-01',31),
(31,1,0,'2013-01-01',31),
(32,1,0,'2013-01-01',31),
(33,1,0,'2013-01-01',31),
(34,1,0,'2013-01-01',31),
(35,1,0,'2013-01-01',31),
(36,1,0,'2013-01-01',31),
(37,1,0,'2013-01-01',31),
(38,1,0,'2013-01-01',31),
(39,1,0,'2013-01-01',31),
(40,1,0,'2013-01-01',31),
(41,1,0,'2013-01-01',31),
(42,1,0,'2013-01-01',31),
(43,1,0,'2013-01-01',31),
(44,1,0,'2013-01-01',31),
(45,1,0,'2013-01-01',31),
(46,1,0,'2013-01-01',31),
(47,1,0,'2013-01-01',31),
(48,1,0,'2013-01-01',31),
(49,1,0,'2013-01-01',31),
(50,1,0,'2013-01-01',31),
(51,1,0,'2013-01-01',31),
(52,1,0,'2013-01-01',31),
(53,1,0,'2013-01-01',31),
(54,1,0,'2013-01-01',31);


INSERT INTO Causes([Name],[Code],[Description],[ModifiedDate],[CreatedDate],[Status],[ModifiedBy],[Level]) VALUES
('Causes',0,'','2013-01-01','2013-01-01',1,0,0);



SET IDENTITY_INSERT dbo.COSTCENTERS ON

INSERT INTO CostCenters([Id],[Name],[Description],[SourceType],[Status]) VALUES
(1,N'ماشین ها','',1,1);

INSERT INTO COSTS([DESCRIPTION],[COSTVALUE],[QUANTITY],[DATE],[STATUS],[CostType],[CostCenter_Id]) VALUES
(N'هزینه ماشین',0,0,'2013-01-01',1,0,1);

INSERT INTO COSTCENTERS([Id],[Name],[Description],[SourceType],[Status]) VALUES
(2,N'اپراتور ها','',2,1);

INSERT INTO COSTS([DESCRIPTION],[COSTVALUE],[QUANTITY],[DATE],[STATUS],[CostType],[CostCenter_Id]) VALUES
(N'هزینه اپراتور',0,0,'2013-01-01',1,0,2);

INSERT INTO COSTCENTERS([Id],[Name],[Description],[SourceType],[Status]) VALUES
(3,N'ایستگاه ها','',3,1);

INSERT INTO COSTS([DESCRIPTION],[COSTVALUE],[QUANTITY],[DATE],[STATUS],[CostType],[CostCenter_Id]) VALUES
(N'هزینه ایستگاه',0,0,'2013-01-01',1,0,3);

INSERT INTO COSTCENTERS([Id],[Name],[Description],[SourceType],[Status]) VALUES
(4,N'فعالیت ها','',4,1);

INSERT INTO COSTS([DESCRIPTION],[COSTVALUE],[QUANTITY],[DATE],[STATUS],[CostType],[CostCenter_Id]) VALUES
(N'هزینه فعالیت',0,0,'2013-01-01',1,0,4);

INSERT INTO COSTCENTERS([Id],[Name],[Description],[SourceType],[Status]) VALUES
(5,N'سربار','',0,1);

INSERT INTO COSTS([DESCRIPTION],[COSTVALUE],[QUANTITY],[DATE],[STATUS],[CostType],[CostCenter_Id]) VALUES
(N'هزینه متفرقه',0,0,'2013-01-01',1,0,5);

SET IDENTITY_INSERT dbo.COSTCENTERS OFF

SET IDENTITY_INSERT dbo.UnitGroups ON
insert into UnitGroups([Id],[ModifiedBy],[Name],[Status]) values
(1,1,N'وزنی',1);

insert into UnitGroups([Id],[ModifiedBy],[Name],[Status]) values
(2,1,N'حجمی',1); 
SET IDENTITY_INSERT dbo.UnitGroups OFF

SET IDENTITY_INSERT dbo.unitsets ON

insert into UnitSets([Id],[Code],[Description],[ModifiedBy],[Status],[UnitGroup_Id]) values
(1,'gr', N'گرم',1,1,1); 
insert into UnitSets([Id],[Code],[Description],[ModifiedBy],[Status],[UnitGroup_Id]) values
(2,'kg', N'کیلوگرم',1,1,1); 
insert into UnitSets([Id],[Code],[Description],[ModifiedBy],[Status],[UnitGroup_Id]) values
(3,'Tn', N'تن',1,1,1); 

insert into UnitSets([Id],[Code],[Description],[ModifiedBy],[Status],[UnitGroup_Id]) values
(4,'cc', N'سی سی',1,1,2); 
insert into UnitSets([Id],[Code],[Description],[ModifiedBy],[Status],[UnitGroup_Id]) values
(5,'Lt', N'لیتر',1,1,2); 
insert into UnitSets([Id],[Code],[Description],[ModifiedBy],[Status],[UnitGroup_Id]) values
(6,'M3', N'متر مکعب',1,1,2); 

SET IDENTITY_INSERT dbo.unitsets OFF


insert into UnitConversions([MinorUnit_Id],[MajorUnit_Id],[Factor],[ModifiedBy],[Status]) values
(1,2,1000,1,1);
insert into UnitConversions([MinorUnit_Id],[MajorUnit_Id],[Factor],[ModifiedBy],[Status]) values
(2,3,1000,1,1);
insert into UnitConversions([MinorUnit_Id],[MajorUnit_Id],[Factor],[ModifiedBy],[Status]) values
(1,3,1000000,1,1);


insert into UnitConversions([MinorUnit_Id],[MajorUnit_Id],[Factor],[ModifiedBy],[Status]) values
(4,5,1000,1,1);
insert into UnitConversions([MinorUnit_Id],[MajorUnit_Id],[Factor],[ModifiedBy],[Status]) values
(5,6,1000,1,1);
insert into UnitConversions([MinorUnit_Id],[MajorUnit_Id],[Factor],[ModifiedBy],[Status]) values
(4,6,1000000,1,1);











-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-----------------------------------------            Bizz             ---------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
use SoheilDb

SET IDENTITY_INSERT dbo.ProductGroups ON
insert into dbo.ProductGroups (Id, Code, Name, ModifiedDate, CreatedDate, [Status], ModifiedBy) values 
(1, 'PG1', N'زانویی', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(2, 'PG2', N'داکت', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(3, 'PG3', N'گروه جدید', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(4, 'PG4', N'یک گروه دیگر', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(5, 'PG5', N'باز هم یک گروه دیگر', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(6, 'PG_', N'خالی', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.ProductGroups OFF

SET IDENTITY_INSERT dbo.Products ON
insert into dbo.Products (Id, Code, Name, ColorNumber, AltColorNumber, ProductGroup_Id, ModifiedDate, CreatedDate, [Status], ModifiedBy) values 
(1, 'Zan_Sam', N'زانویی سمند',((20*256+200)*256+200) ,0,1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(2, 'Zan_L90', N'زانویی L90',((23*256+133)*256+198) ,0, 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(3, 'RD_Sam', N'داکت راست سمند', ((128*256+20)*256+200),0,2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(4, 'LD_Sam', N'داکت چپ سمند', ((100*256+132)*256+128),0,2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(5, 'Zan_prd', N'زانویی پراید',((120*256+200)*256+20) ,0,1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(6, 'Zan_pjo', N'زانویی پژو',((223*256+133)*256+198) ,0, 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(7, 'RD_prd', N'داکت راست پراید', ((128*256+20)*256+200),0,2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(8, 'RD_pjo', N'داکت راست پژو', ((100*256+132)*256+128),0,2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(9, 'LD_prd', N'داکت چپ پراید', ((255*256+200)*256+128),0,2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(10, 'LD_pjo', N'داکت چپ پژو', ((200*256+255)*256+128),0,2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(11, 'Prod1', N'محصول جدید 1',((140*256+200)*256+120) ,0,3, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(12, 'Prod2', N'محصول جدید 2',((20*256+220)*256+120) ,0,3, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(13, 'Prod3', N'محصول جدید 3',((20*256+200)*256+20) ,0,3, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(14, 'Prod4', N'محصول جدید 4',((140*256+100)*256+120) ,0,3, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(15, 'Prod5', N'محصول جدید 5',((192*256+100)*256+20) ,0,3, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(16, 'ProdNew1', N'محصول جدید دیگر 1',((255*256+160)*256+120) ,0,4, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(17, 'ProdNew2', N'محصول جدید دیگر 2',((255*256+60)*256+120) ,0,4, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(18, 'ProdNew3', N'محصول جدید دیگر 3',((255*256+160)*256+20) ,0,4, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(19, 'ProdNew4', N'محصول جدید دیگر 4',((255*256+60)*256+20) ,0,4, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(20, 'ProdNew5', N'محصول جدید دیگر 5',((200*256+160)*256+120) ,0,4, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(21, 'ProdOther1', N'باز هم محصول جدید دیگر 1',((90*256+160)*256+255) ,0,5, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(22, 'ProdOther2', N'باز هم محصول جدید دیگر 2',((160*256+160)*256+220) ,0,5, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(23, 'ProdOther3', N'باز هم محصول جدید دیگر 3',((50*256+210)*256+255) ,0,5, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(24, 'ProdOther4', N'باز هم محصول جدید دیگر 4',((120*256+160)*256+255) ,0,5, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(25, 'ProdOther5', N'باز هم محصول جدید دیگر 5',((120*256+160)*256+220) ,0,5, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(26, 'Prod_', N'خالی',((255*256+184)*256+69) ,0,1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.Products OFF

SET IDENTITY_INSERT dbo.Defections ON
insert into dbo.Defections (Id, Code, Name, ModifiedDate, CreatedDate, [Status], ModifiedBy) values 
(1, 'Df1', N'خال خال', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(2, 'Df2', N'دانه دار', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(3, 'Df3', N'معیوب', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(4, 'Df4', N'پارگی', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(5, 'Df5', N'سوراخدار', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.Defections OFF

SET IDENTITY_INSERT dbo.ProductDefections ON
insert into dbo.ProductDefections (Id, Product_Id, Defection_Id) values 
(1, 1, 1),
(2, 1, 2),
(3, 1, 3),
(4, 1, 4),
(5, 1, 5),
(6, 2, 1),
(7, 2, 2),
(8, 2, 3),
(9, 2, 4),
(10, 2, 5),
(11, 3, 1),
(12, 3, 2),
(13, 4, 3),
(14, 4, 4),
(15, 5, 5),
(16, 5, 1),
(17, 5, 2),
(18, 6, 3),
(19, 6, 4),
(20, 7, 5),
(21, 7, 1),
(22, 7, 2),
(23, 8, 3),
(24, 8, 4),
(25, 9, 5),
(26, 9, 1),
(27, 9, 2),
(28, 10, 3),
(29, 10, 4),
(30, 10, 5);
SET IDENTITY_INSERT dbo.ProductDefections OFF


SET IDENTITY_INSERT dbo.Reworks ON
insert into dbo.Reworks (Id, Code, Name, ModifiedDate, CreatedDate, [Status], ModifiedBy) values 
(1, 'Rew1', N'دوباره کاری نوع 1', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(2, 'Rew2', N'دوباره کاری نوع 2', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(3, 'RHole', N'سوراخ شدگی محصول', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(4, 'RNew', N'دوباره کاری دیگر', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(5, 'ROther', N'باز هم دوباره کاری دیگر', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(6, 'Rew_', N'خالی', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.Reworks OFF

SET IDENTITY_INSERT dbo.ProductReworks ON
insert into dbo.ProductReworks (Id, Code, Name, Product_Id, Rework_Id, [Status], ModifiedBy, Inventory) values 
(1, 'Pr1Main', N'زانویی سمند', 1, NULL, 1, 1,0),
(2, 'Pr1R1', N'زانویی سمند دوباره کاری 1', 1, 1, 1, 1,0),
(3, 'Pr1R2', N'زانویی سمند دوباره کاری 2', 1, 2, 1, 1,0),
(4, 'Pr2Main', N'زانویی L90', 2, NULL, 1, 1,0),
(5, 'Pr2R1', N'زانویی L90 دوباره کاری 1', 2, 1, 1, 1,0),
(6, 'Pr2R3', N'زانویی L90 دوباره کاری 3', 2, 3, 1, 1,0),
(7, 'Pr3Main', N'داکت راست سمند', 3, NULL, 1, 1,0),
(8, 'Pr4Main', N'داکت چپ سمند', 4, NULL, 1, 1,0),
(9, 'Zan_pjo', N'زانویی پژو',6, NULL, 1, 1,0),
(10, 'Zan_pjo', N'زانویی پژو سوراخ',6, 3, 1, 1,0),
(11, 'RD_prd', N'داکت راست پراید',7, NULL, 1, 1,0),
(12, 'RD_prd', N'داکت راست پراید نوع1',7, 1, 1, 1,0),
(13, 'RD_prd', N'داکت راست پراید نوع2',7, 2, 1, 1,0),
(14, 'RD_pjo', N'داکت راست پژو',8, NULL, 1, 1,0),
(15, 'LD_prd', N'داکت چپ پراید', 9, NULL, 1, 1,0),
(16, 'LD_prd', N'داکت چپ پراید خالخالی', 9, 4, 1, 1,0),
(17, 'LD_pjo', N'داکت چپ پژو',10, NULL, 1, 1,0),
(18, 'Prod1', N'محصول جدید 1',11, NULL, 1, 1,0),
(19, 'Prod1', N'محصول جدید 1سفیدک',11, 4, 1, 1,0),
(20, 'Prod1', N'محصول جدید 1دانه دار',11, 5, 1, 1,0),
(21, 'Prod2', N'محصول جدید 2',12, NULL, 1, 1,0),
(22, 'Prod2', N'محصول جدید خراب2',12, 5, 1, 1,0),
(23, 'Prod3', N'محصول جدید 3',13, NULL, 1, 1,0),
(24, 'Prod3', N'محصول جدید مشکل دار3',13, 3, 1, 1,0),
(25, 'Prod4', N'محصول جدید 4',14, NULL, 1, 1,0),
(26, 'Prod4', N'محصول جدید 4شل',14, 1, 1, 1,0),
(27, 'Prod4', N'محصول جدید 4سفت',14, 2, 1, 1,0),
(28, 'Prod4', N'محصول جدید 4کج',14, 3, 1, 1,0),
(29, 'Prod5', N'محصول جدید 5',15, NULL, 1, 1,0),
(30, 'Prod5', N'محصول جدید 5خراب',15, 3, 1, 1,0),
(31, 'Prod5', N'محصول جدید 5دوباره کاری دارد',15, 5, 1, 1,0),
(32, 'ProdNew1', N'محصول جدید دیگر 1',16, NULL, 1, 1,0),
(33, 'ProdNew1', N'محصول جدید دیگر 1مشکل دارد',16, 3, 1, 1,0),
(34, 'ProdNew1', N'محصول جدید دیگر 1بدون پیچ',16, 4, 1, 1,0),
(35, 'ProdNew2', N'محصول جدید دیگر 2',17, NULL, 1, 1,0),
(36, 'ProdNew2', N'محصول جدید دیگر 2شل',17, 4, 1, 1,0),
(37, 'ProdNew2', N'محصول جدید دیگر کج2',17, 5, 1, 1,0),
(38, 'ProdNew3', N'محصول جدید دیگر 3',18, NULL, 1, 1,0),
(39, 'ProdNew3', N'محصول جدید دیگر 3شل',18, 1, 1, 1,0),
(40, 'ProdNew4', N'محصول جدید دیگر 4',19, NULL, 1, 1,0),
(41, 'ProdNew4', N'محصول جدید دیگر 4شل',19, 3, 1, 1,0),
(42, 'ProdNew5', N'محصول جدید دیگر 5',20, NULL, 1, 1,0),
(43, 'ProdOther1', N'باز هم محصول جدید دیگر 1',21, NULL, 1, 1,0),
(44, 'ProdOther1', N'باز هم محصول جدید دیگر 1خرابی1',21, 4, 1, 1,0),
(45, 'ProdOther1', N'باز هم محصول جدید دیگر 1خرابی2',21, 5, 1, 1,0),
(46, 'ProdOther2', N'باز هم محصول جدید دیگر 2',22, NULL, 1, 1,0),
(47, 'ProdOther2', N'باز هم محصول جدید دیگر 2خرابی1',22, 3, 1, 1,0),
(48, 'ProdOther2', N'باز هم محصول جدید دیگر 2خرابی2',22, 4, 1, 1,0),
(49, 'ProdOther2', N'باز هم محصول جدید دیگر 2خرابی3',22, 5, 1, 1,0),
(50, 'ProdOther3', N'باز هم محصول جدید دیگر 3',23, NULL, 1, 1,0),
(51, 'ProdOther3', N'باز هم محصول جدید دیگر 3خراب',23, 2, 1, 1,0),
(52, 'ProdOther5', N'باز هم محصول جدید دیگر 5',25, NULL, 1, 1,0),
(53, 'ProdOther5', N'باز هم محصول جدید دیگر 5خراب',25, 1, 1, 1,0),
(54, 'Zan_prd', N'زانویی پراید',5, NULL, 1, 1,0);
SET IDENTITY_INSERT dbo.ProductReworks OFF


SET IDENTITY_INSERT dbo.Stations ON
insert into dbo.Stations (Id, [Index], Code, Name, ModifiedDate, CreatedDate, [Status], ModifiedBy) values 
(1,0, 'BM01', N'BM01', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(2,1, 'BM02', N'BM02', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(3,2, 'BM03', N'BM03', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(4,3, 'BM04', N'BM04', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(5,4, 'BM05', N'BM05', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(6,5, 'BM06', N'BM06', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(7,6, 'BM07', N'BM07', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(8,7, 'BM08', N'BM08', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(9,8, 'Aux', N'کمکی', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.Stations OFF

SET IDENTITY_INSERT dbo.MachineFamilies ON
insert into dbo.MachineFamilies (Id, Code, Name, ModifiedDate, CreatedDate, [Status], ModifiedBy) values 
(1, 'MachFam1', N'تزریق پلی اتیلن', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(2, 'MachFam2', N'تزریق PVC', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(3, 'MachFam3', N'دستگاه برش', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(4, 'MachFam_', N'خالی', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.MachineFamilies OFF

SET IDENTITY_INSERT dbo.Machines ON
insert into dbo.Machines (Id, Code,  Name,HasOEE, MachineFamily_Id, ModifiedDate, CreatedDate, [Status], ModifiedBy) values 
(1, 'PE1', N'PEBM01', 'True', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(2, 'PE2', N'PEBM02', 'True',1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(3, 'PE3', N'PEBM03', 'True',1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(4, 'PE4', N'PEBM04', 'True',1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(5, 'PE5', N'PEBM05', 'True',1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(6, 'PVC1', N'PVCBM06', 'True',2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(7, 'PVC2', N'PVCBM07', 'True',2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(8, 'PVC3', N'PVCBM08', 'True',2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(9, 'Cut1', N'کاتر برقی', 'True',3, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(10, 'Cut2', N'فرز','True',3, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(11, 'PE_', N'خالی','True',1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.Machines OFF

SET IDENTITY_INSERT dbo.Parts ON
insert into dbo.Parts (Id, Code,  Name, [Description], [Status], ModifiedDate, ModifiedBy) values 
(1, '1', 'Part1', 'Part1', 1, {fn CURRENT_TIMESTAMP()}, 1),
(2, '2', 'Part2', 'Part2', 1, {fn CURRENT_TIMESTAMP()}, 1),
(3, '3', 'Part3', 'Part3', 1, {fn CURRENT_TIMESTAMP()}, 1),
(4, '4', 'Part4', 'Part4', 1, {fn CURRENT_TIMESTAMP()}, 1),
(5, '5', 'Part5', 'Part5', 1, {fn CURRENT_TIMESTAMP()}, 1),
(6, '6', 'Part6', 'Part6', 0, {fn CURRENT_TIMESTAMP()}, 1),
(7, '7', 'Part7', 'Part7', 2, {fn CURRENT_TIMESTAMP()}, 1),
(8, '8', 'Part8', 'Part8', 1, {fn CURRENT_TIMESTAMP()}, 1),
(9, '9', 'Part9', 'Part9', 1, {fn CURRENT_TIMESTAMP()}, 1);
SET IDENTITY_INSERT dbo.Parts OFF

SET IDENTITY_INSERT dbo.Maintenances ON
insert into dbo.Maintenances (Id, Code,  Name, [Description], [Status], ModifiedDate, ModifiedBy) values 
(1, '1', 'PM.1', 'Maintenance #1', 1, {fn CURRENT_TIMESTAMP()}, 1),
(2, '2', 'PM.2', 'Maintenance #2', 1, {fn CURRENT_TIMESTAMP()}, 1),
(3, '3', 'PM.3', 'Maintenance #3', 1, {fn CURRENT_TIMESTAMP()}, 1),
(4, '4', 'PM.4', 'Maintenance #4', 1, {fn CURRENT_TIMESTAMP()}, 1),
(5, '5', 'PM.5', '', 1, {fn CURRENT_TIMESTAMP()}, 1),
(6, '6', 'PM.6', 'Maintenance #6', 0, {fn CURRENT_TIMESTAMP()}, 1),
(7, '7', 'PM.7', 'Maintenance #7', 2, {fn CURRENT_TIMESTAMP()}, 1),
(8, '8', 'PM.8', 'Maintenance #8', 1, {fn CURRENT_TIMESTAMP()}, 1),
(9, '9', 'PM.9', 'Maintenance #9', 1, {fn CURRENT_TIMESTAMP()}, 1);
SET IDENTITY_INSERT dbo.Maintenances OFF

SET IDENTITY_INSERT dbo.ActivityGroups ON
insert into dbo.ActivityGroups (Id, Code, Name, ModifiedDate, CreatedDate, [Status]) values 
(1, 'ActGrp1', N'تولید', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1),
(2, 'ActGrp2', N'پردازش', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1),
(3, 'ActGrp_', N'خالی', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1);
SET IDENTITY_INSERT dbo.ActivityGroups OFF

SET IDENTITY_INSERT dbo.Activities ON
insert into dbo.Activities (Id, Code, Name, ActivityGroup_Id, ModifiedDate, CreatedDate, [Status], ModifiedBy) values 
(1, 'Act1', N'تولید پلی اتیلن', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(2, 'Act2', N'تولید PVC', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(3, 'Act3', N'برش', 2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(4, 'Act4', N'پلیسه گیری', 2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(5, 'Act5', N'برچسب زنی', 2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(6, 'Act_', N'خالی', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.Activities OFF

SET IDENTITY_INSERT dbo.FPCs ON
insert into dbo.FPCs (Id, Code, Name, IsDefault, Product_Id, ModifiedDate, CreatedDate, [Status], ModifiedBy) values 
(1, 'Pr1Set1', N'زانویی سمند 1',				'True',	1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(2, 'Pr1Set2', N'زانویی سمند 2',				'False',1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(3, 'Pr2Set1', N'زانویی L90 1',					'True',	2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(4, 'Pr2Set2_', N'خالی',						'False',2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(5, 'Zan_prd', N'زانویی پراید' ,				'True',	5, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(6, 'Zan_pjo', N'زانویی پژو',					'True',	6, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(7, 'RD_prd', N'داکت راست پراید',				'True',	7, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(8, 'RD_pjo', N'داکت راست پژو',				'True',	8, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(9, 'LD_prd', N'داکت چپ پراید',				'True',	9, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(10, 'LD_pjo', N'داکت چپ پژو',					'True',10, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(11, 'Prod1', N'محصول جدید 1',					'True',11, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(12, 'Prod2', N'محصول جدید 2',					'True',12, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(13, 'Prod3', N'محصول جدید 3',					'True',13, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(14, 'Prod4', N'محصول جدید 4',					'True',14, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(15, 'Prod5', N'محصول جدید 5',					'True',15, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(16, 'ProdNew1', N'محصول جدید دیگر 1',		'True',16, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(17, 'ProdNew2', N'محصول جدید دیگر 2',		'True',17, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(18, 'ProdNew3', N'محصول جدید دیگر 3',		'True',18, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(19, 'ProdNew4', N'محصول جدید دیگر 4',		'True',19, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(20, 'ProdNew5', N'محصول جدید دیگر 5',		'True',20, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(21, 'ProdOther1',N'باز هم محصول جدید دیگر1','True',21, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(22, 'ProdOther2',N'باز هم محصول جدید دیگر2','True',22, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(23, 'ProdOther3',N'باز هم محصول جدید دیگر3','True',23, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(24, 'ProdOther4',N'باز هم محصول جدید دیگر4','True',24, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(25, 'ProdOther5',N'باز هم محصول جدید دیگر5','True',25, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.FPCs OFF

SET IDENTITY_INSERT dbo.Operators ON
insert into dbo.Operators (Id, Name, Code, Score, CreatedDate, ModifiedDate, [Status], ModifiedBy, Age, Sex) values 
(1, N'پژمان مسعودی', '001', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(2, N'زهرا کرمی زاده', '002', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'False'),
(3, N'مهدی علیدوست', '003', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(4, N'سجاد سلطانیان', '004', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(5, N'هوشنگ هدایت', '005', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(6, N'فریدون هاشمی', '006', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(7, N'سهیلا سعادتیان', '007', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'False'),
(8, N'فرهاد حاج مرادی', '008', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(9, N'پشمان مجعودی', '009', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(10, N'پجمان مسفودی', '011', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(11, N'پچمان مستودی', '021', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(12, N'سید جلاد نرمی زاده', '102', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(13, N'مهلی علیداست', '103', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(14, N'سعاد سلصانیان', '204', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(15, N'هوبنگ هتایت', '015', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(16, N'فرییون هاخمی', '106', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(17, N'سهیمه سقدتیان', '237', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'False'),
(18, N'فرجاد حاج مقادی', '128', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(19, N'فریاد حاج مدادی', '328', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(20, N'فرخاد حاج منادی', '458', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True');
SET IDENTITY_INSERT dbo.Operators OFF

SET IDENTITY_INSERT dbo.States ON
insert into dbo.States (Id, Code,Name, StateTypeNr, X, Y, FPC_Id, OnProductRework_Id) values 
('1',N'شروع',N'شروع','0','50','50','1',NULL),
('2','Lvl1',N'مرحله اول','1','350','70','1','1'),
('3','Lvl2',N'مرحله دوم','1','50','160','1','1'),
('4','Lvl1',N'دوباره کاری نوع 2','3','400','300','1','2'),
('5','Lvl1',N'دوباره کاری نوع 1','3','50','400','1','3'),
('6','Lvl1',N'زانویی سمند','2','400','400','1',NULL),
('7','---',N'شروع','0','176','23','3',NULL),
('8','---',N'تولید عادی','2','405','491','3',NULL),
('9','Pr2R1',N'زانویی L90 دوباره کاری 1','3','40','299','3','5'),
('10','Pr2R3',N'زانویی L90 دوباره کاری 3','3','208','293','3','6'),
('11','---',N'شروع','0','158','36','5',NULL),
('12','---',N'تولید عادی','2','302','495','5',NULL),
('13','---',N'شروع','0','261','13','6',NULL),
('14','---',N'تولید عادی','2','435','586','6',NULL),
('15','Zan_pjo',N'زانویی پژو سوراخ','3','543','384','6','10'),
('16','---',N'شروع','0','50','50','7',NULL),
('17','---',N'تولید عادی','2','651','292','7',NULL),
('18','RD_prd',N'داکت راست پراید نوع1','3','205','246','7','12'),
('19','RD_prd',N'داکت راست پراید نوع2','3','41','307','7','13'),
('20','---',N'شروع','0','50','50','8',NULL),
('21','---',N'تولید عادی','2','300','50','8',NULL),
('22','---',N'شروع','0','50','50','9',NULL),
('23','---',N'تولید عادی','2','89','541','9',NULL),
('24','LD_prd',N'داکت چپ پراید خالخالی','3','218','337','9','16'),
('25','---',N'شروع','0','50','50','10',NULL),
('26','---',N'تولید عادی','2','300','50','10',NULL),
('27','---',N'شروع','0','50','50','11',NULL),
('28','---',N'تولید عادی','2','108','536','11',NULL),
('29','Prod1',N'محصول جدید 1سفیدک','3','4','233','11','19'),
('30','Prod1',N'محصول جدید 1دانه دار','3','160','253','11','20'),
('31','---',N'شروع','0','50','50','12',NULL),
('32','---',N'تولید عادی','2','80','456','12',NULL),
('33','Prod2',N'محصول جدید خراب2','3','297','221','12','22'),
('34','---',N'شروع','0','50','50','13',NULL),
('35','---',N'تولید عادی','2','307','539','13',NULL),
('36','Prod3',N'محصول جدید مشکل دار3','3','25','217','13','24'),
('37','---',N'شروع','0','50','50','14',NULL),
('38','---',N'تولید عادی','2','122','592','14',NULL),
('39','Prod4',N'محصول جدید 4شل','3','1','270','14','26'),
('40','Prod4',N'محصول جدید 4سفت','3','102','426','14','27'),
('41','Prod4',N'محصول جدید 4کج','3','200','277','14','28'),
('42','---',N'شروع','0','50','50','15',NULL),
('43','---',N'تولید عادی','2','423','532','15',NULL),
('44','Prod5',N'محصول جدید 5خراب','3','84','351','15','30'),
('45','Prod5',N'محصول جدید 5دوباره کاری دارد','3','22','283','15','31'),
('46','---',N'شروع','0','50','50','16',NULL),
('47','---',N'تولید عادی','2','300','50','16',NULL),
('48','ProdNew1',N'محصول جدید دیگر 1مشکل دارد','3','50','250','16','33'),
('49','ProdNew1',N'محصول جدید دیگر 1بدون پیچ','3','100','300','16','34'),
('50','---',N'شروع','0','50','50','17',NULL),
('51','---',N'تولید عادی','2','369','491','17',NULL),
('52','ProdNew2',N'محصول جدید دیگر 2شل','3','50','250','17','36'),
('53','ProdNew2',N'محصول جدید دیگر کج2','3','100','300','17','37'),
('54','---',N'شروع','0','50','50','18',NULL),
('55','---',N'تولید عادی','2','342','522','18',NULL),
('56','ProdNew3',N'محصول جدید دیگر 3شل','3','50','250','18','39'),
('57','---',N'شروع','0','50','50','19',NULL),
('58','---',N'تولید عادی','2','333','580','19',NULL),
('59','ProdNew4',N'محصول جدید دیگر 4شل','3','50','250','19','41'),
('60','---',N'شروع','0','50','50','20',NULL),
('61','---',N'تولید عادی','2','300','50','20',NULL),
('62','---',N'شروع','0','50','50','21',NULL),
('63','---',N'تولید عادی','2','300','50','21',NULL),
('64','ProdOther1',N'باز هم محصول جدید دیگر 1خرابی1','3','5','251','21','44'),
('65','ProdOther1',N'باز هم محصول جدید دیگر 1خرابی2','3','136','501','21','45'),
('66','---',N'شروع','0','50','50','22',NULL),
('67','---',N'تولید عادی','2','160','33','22',NULL),
('68','ProdOther2',N'باز هم محصول جدید دیگر 2خرابی1','3','127','386','22','47'),
('69','ProdOther2',N'باز هم محصول جدید دیگر 2خرابی2','3','333','296','22','48'),
('70','ProdOther2',N'باز هم محصول جدید دیگر 2خرابی3','3','141','536','22','49'),
('71','---',N'شروع','0','50','50','23',NULL),
('72','---',N'تولید عادی','2','300','50','23',NULL),
('73','ProdOther3',N'باز هم محصول جدید دیگر 3خراب','3','-69','510','23','51'),
('74','---',N'شروع','0','50','50','24',NULL),
('75','---',N'تولید عادی','2','300','50','24',NULL),
('76','---',N'شروع','0','50','50','25',NULL),
('77','---',N'تولید عادی','2','300','50','25',NULL),
('78','ProdOther5',N'باز هم محصول جدید دیگر 5خراب','3','50','250','25','53'),
('79','3',N'مرحله 3','1','159','432','3',NULL),
('80','2',N'مرحله 2','1','273','122','3',NULL),
('81','1',N'مرحله 1','1','117','106','3',NULL),
('82','1',N'مرحله 1','1','159','114','5',NULL),
('83','2',N'مرحله 2','1','57','312','5',NULL),
('84','32',N'مرحله 34','1','272','339','5',NULL),
('85','A',N'مرحلهA','1','192','103','6',NULL),
('86','C',N'مرحله C','1','336','97','6',NULL),
('87','B',N'مرحله بB','1','249','205','6',NULL),
('88','Y--',N'مرحله YY','1','243','341','6',NULL),
('89','Z','ZZZZ','1','427','397','6',NULL),
('90','X',N'مرحله XX','1','111','258','6',NULL),
('91','---',N'مرحله 1','1','184','134','7',NULL),
('92','---',N'مرحله 2','1','535','67','7',NULL),
('93','---',N'مرحلهrework','1','282','389','7',NULL),
('94','---',N'مرحله1','1','114','163','8',NULL),
('95','---',N'مرحله ب2','1','525','162','8',NULL),
('96','---',N'مرحله rew','1','339','257','8',NULL),
('97','3',N'مرحله 3','1','258','451','9',NULL),
('98','2',N'مرحله 2','1','83','312','9',NULL),
('99','1',N'مرحله 1','1','80','186','9',NULL),
('100','---',N'مرحله بدون نام','1','68','175','10',NULL),
('101','1',N'مرحله1','1','123','133','11',NULL),
('102','r2',N'مرحلهr2','1','225','373','11',NULL),
('103','r1',N'مرحله r1','1','8','367','11',NULL),
('104','1',N'مرحله 1','1','124','164','12',NULL),
('105','r',N'مرحله r','1','289','379','12',NULL),
('106','---',N'مرحله بدون نام','1','392','156','13',NULL),
('107','---',N'مرحله بدون نام','1','67','381','13',NULL),
('108','---1',N'مرحله ب1','1','99','130','14',NULL),
('109','r',N'مرحل1er','1','30','520','14',NULL),
('110','---',N'مرحله بدون نام','1','274','526','14',NULL),
('111','1',N'مرحله1','1','375','54','15',NULL),
('112','2',N'مرحله2','1','307','201','15',NULL),
('113','---',N'مرحله بدون نام','1','100','493','15',NULL),
('114','1X',N'مرحله یه ضرب','1','288','173','16',NULL),
('115',N'کامل',N'مرحله کامل','1','340','257','17',NULL),
('116',N'فعلالبت',N'مرحله بدون فعالیت','1','85','468','17',NULL),
('117',N'فع1---',N'مرحله بدون فعالیت1','1','309','226','18',NULL),
('118',N'فع2---',N'مرحله بدون فعالیت2','1','35','333','18',NULL),
('119',N'ماش1---',N'مرحله بدون ماشین1','1','343','246','19',NULL),
('120',N'ماش2---',N'مرحله بدون ماشین2','1','60','384','19',NULL),
('121',N'قع---',N'مرحله بدون قعالبت','1','337','166','20',NULL),
('122',N'ماش---',N'مرحله بدون ماشین','1','112','60','20',NULL),
('123',N'ایس---',N'مرحله بدون ایستگاه','1','309','407','20',NULL),
('124',N'کالم---',N'مرحله کامل','1','38','274','20',NULL),
('125','loop1','loop1','1','390','440','22',NULL),
('126','loop2','loop2','1','-25','432','22',NULL),
('127','rev',N'مرحله reverse','1','297','87','22',NULL),
('128','2REV',N'مرحله دوطرفه','1','225','347','23',NULL),
('129',N'دوطرفه مرحله',N'مرحله دوطرفه','1','313','237','25',NULL),
('130',N'دوطرفهدوم مرحله',N'مرحلهدوطرفه دوم','1','313','459','25',NULL),
('131','island',N'مرحله جزیره','1','60','356','25',NULL);
SET IDENTITY_INSERT dbo.States OFF

SET IDENTITY_INSERT dbo.Connectors ON
insert into dbo.Connectors (Id, StartState_Id, EndState_Id, HasBuffer) values 
('1','1','2','True'),
('2','2','4','False'),
('3','2','6','True'),
('4','4','3','False'),
('5','3','6','True'),
('6','3','5','True'),
('7','5','2','False'),
('16','7','81','True'),
('17','7','80','True'),
('18','80','10','False'),
('19','10','79','False'),
('20','79','8','True'),
('21','80','8','False'),
('22','81','9','True'),
('23','9','79','True'),
('39','11','82','False'),
('40','82','83','True'),
('41','83','84','False'),
('42','84','12','False'),
('43','82','84','True'),
('65','13','85','True'),
('66','13','86','True'),
('67','86','87','False'),
('68','85','87','True'),
('69','85','90','False'),
('70','87','90','True'),
('71','90','88','True'),
('72','87','88','False'),
('73','88','89','True'),
('74','86','89','True'),
('75','89','14','False'),
('84','16','91','True'),
('85','91','18','False'),
('86','91','19','True'),
('87','91','92','True'),
('88','92','17','True'),
('89','18','93','False'),
('90','19','93','True'),
('91','93','92','False'),
('92','20','94','True'),
('93','94','95','False'),
('94','95','21','True'),
('95','96','21','False'),
('102','22','99','True'),
('103','99','98','True'),
('104','98','24','True'),
('105','24','97','False'),
('106','97','23','True'),
('107','98','23','True'),
('108','25','100','True'),
('109','100','26','False'),
('117','27','101','True'),
('118','101','29','False'),
('119','29','103','True'),
('120','103','28','True'),
('121','101','30','True'),
('122','30','102','True'),
('123','102','28','True'),
('124','101','28','True'),
('130','31','104','True'),
('131','104','33','False'),
('132','33','105','True'),
('133','105','32','True'),
('134','104','32','True'),
('135','34','106','False'),
('136','106','35','True'),
('137','106','36','True'),
('138','36','107','False'),
('139','107','35','True'),
('140','37','108','True'),
('141','108','38','False'),
('142','108','39','True'),
('143','108','40','True'),
('144','108','41','False'),
('145','39','109','True'),
('146','40','109','True'),
('147','40','110','False'),
('148','110','38','False'),
('149','41','110','True'),
('150','109','38','True'),
('153','42','111','True'),
('154','111','43','True'),
('155','111','112','True'),
('156','112','43','False'),
('157','112','44','True'),
('158','112','45','True'),
('159','45','113','True'),
('160','44','113','True'),
('161','113','43','False'),
('162','46','114','True'),
('163','114','47','True'),
('164','50','115','True'),
('165','115','51','False'),
('166','115','52','True'),
('167','115','53','True'),
('168','52','116','True'),
('169','53','116','True'),
('170','116','51','True'),
('171','54','117','True'),
('172','117','56','True'),
('173','117','55','True'),
('174','56','118','False'),
('175','118','55','True'),
('176','57','119','True'),
('177','119','59','False'),
('178','59','120','True'),
('179','120','58','True'),
('180','119','58','True'),
('181','60','124','False'),
('182','124','122','True'),
('183','122','121','True'),
('184','121','123','True'),
('185','123','61','True'),
('186','62','64','False'),
('187','64','63','True'),
('188','63','65','False'),
('189','66','125','True'),
('190','125','68','True'),
('191','68','126','False'),
('192','126','70','True'),
('193','70','125','True'),
('194','66','67','False'),
('195','67','127','True'),
('196','127','69','False'),
('197','71','128','True'),
('198','128','73','False'),
('199','73','128','True'),
('200','128','72','False'),
('201','74','75','True'),
('202','76','129','False'),
('203','129','130','True'),
('204','130','129','True'),
('205','78','77','False');
SET IDENTITY_INSERT dbo.Connectors OFF

SET IDENTITY_INSERT dbo.StationMachines ON
insert into dbo.StationMachines (Id, Station_Id, Machine_Id, [Status]) values 
(1, 1, 1, 1),
(2, 2, 2, 1),
(3, 3, 3, 1),
(4, 4, 4, 1),
(5, 5, 5, 1),
(6, 6, 6, 1),
(7, 7, 7, 1),
(8, 8, 8, 1),
(9, 9, 9, 1),
(10, 9, 10, 1),
(11, 9, 11, 1),
(12, 1, 2, 1),
(13, 1, 3, 1),
(14, 2, 3, 1),
(15, 2, 4, 1);
SET IDENTITY_INSERT dbo.StationMachines OFF

SET IDENTITY_INSERT dbo.StateStations ON
insert into dbo.StateStations (Id, State_Id, Station_Id, IsDefault) values 
(1,2,1,1),
(2,2,2,0),
(3,2,9,0),
(4,3,1,1),
(5,79,2,1),
(6,79,3,0),
(7,80,2,1),
(8,81,1,1),
(9,82,2,1),
(10,83,3,1),
(11,84,5,1),
(12,84,6,0),
(13,82,9,0),
(14,85,1,1),
(15,85,2,0),
(16,86,2,0),
(17,86,3,0),
(18,86,8,1),
(19,86,9,0),
(20,87,1,0),
(21,87,3,1),
(22,88,3,0),
(23,88,4,1),
(24,89,4,0),
(25,89,6,1),
(26,90,2,1),
(27,90,3,0),
(28,91,1,1),
(29,92,8,1),
(30,93,9,0),
(31,94,1,0),
(32,95,8,0),
(33,96,9,0),
(34,97,7,0),
(35,98,6,0),
(36,98,7,0),
(37,99,6,0),
(38,99,8,0),
(39,99,7,0),
(40,100,8,0),
(41,101,3,0),
(42,102,9,0),
(43,103,9,0),
(44,104,4,0),
(45,105,9,0),
(46,106,5,0),
(47,107,9,0),
(48,108,4,0),
(49,108,5,0),
(50,109,9,0),
(51,110,9,0),
(52,111,6,0),
(53,112,7,0),
(54,113,9,0),
(55,114,1,0),
(56,115,5,0),
(57,116,9,0),
(58,117,1,0),
(59,118,9,0),
(60,119,1,0),
(61,120,9,0),
(62,121,1,0),
(63,121,2,0),
(64,122,2,0),
(65,122,3,0),
(66,122,1,0),
(67,124,4,0),
(68,124,1,0),
(69,2,6,0);
SET IDENTITY_INSERT dbo.StateStations OFF

SET IDENTITY_INSERT dbo.StateStationActivities ON
insert into dbo.StateStationActivities (Id, CycleTime, ManHour, StateStation_Id, Activity_Id, IsMany, IsInJob, IsPrimaryOutput) values 
(1,60,2,1,1, 'True', 'True', 'True'),
(2,90,1,1,2, 'False', 'False', 'False'),
(3,60,2,2,1, 'True', 'True', 'True'),
(4,85,1,2,2, 'False', 'False', 'True'),
(5,40,1,3,3, 'True', 'False', 'True'),
(6,50,1,3,4, 'False', 'False', 'True'),
(7,60,1,3,5, 'True', 'False', 'True'),
(8,60,3,4,3, 'True', 'True', 'True'),
(9,110,1,4,4, 'True', 'False', 'True'),
(10,60,1,5,2, 'True', 'True', 'True'),
(11,60,1,6,1, 'True', 'True', 'True'),
(12,60,1,7,1, 'False', 'True', 'True'),
(13,60,1,8,1, 'True', 'True', 'True'),
(14,60,1,9,1, 'True', 'True', 'True'),
(15,60,1,10,2, 'True', 'True', 'True'),
(16,60,1,11,1, 'True', 'True', 'True'),
(17,60,1,12,2, 'False', 'True', 'True'),
(18,60,1,13,1, 'True', 'True', 'True'),
(19,60,1,14,1, 'True', 'True', 'True'),
(20,60,1,21,2, 'True', 'True', 'True'),
(21,60,1,15,2, 'False', 'True', 'True'),
(22,60,1,16,1, 'True', 'True', 'True'),
(23,60,1,16,2, 'True', 'False', 'True'),
(24,60,1,16,5, 'True', 'False', 'True'),
(25,60,1,17,2, 'False', 'True', 'True'),
(26,60,1,17,3, 'True', 'False', 'True'),
(27,60,1,17,4, 'False', 'False', 'True'),
(28,60,1,18,4, 'True', 'True', 'True'),
(29,60,1,18,2, 'True', 'False', 'True'),
(30,60,1,19,3, 'True', 'True', 'True'),
(31,60,1,19,1, 'True', 'False', 'True'),
(32,60,1,20,1, 'False', 'True', 'True'),
(33,60,1,20,2, 'True', 'False', 'True'),
(34,60,1,22,3, 'False', 'True', 'True'),
(35,60,1,22,1, 'True', 'False', 'True'),
(36,60,1,23,4, 'False', 'True', 'True'),
(37,60,1,23,1, 'False', 'False', 'True'),
(38,60,1,24,3, 'True', 'True', 'True'),
(39,60,1,25,4, 'True', 'True', 'True'),
(40,60,1,26,3, 'True', 'True', 'True'),
(41,60,1,27,4, 'False', 'True', 'True'),
(42,60,1,28,1, 'True', 'True', 'True'),
(43,60,1,29,1, 'True', 'True', 'True'),
(44,60,1,30,3, 'True', 'True', 'True'),
(45,60,1,31,1, 'True', 'True', 'True'),
(46,60,1,32,1, 'True', 'True', 'True'),
(47,60,1,33,3, 'False', 'True', 'True'),
(48,333,1,34,1, 'True', 'True', 'True'),
(49,222,1,35,1, 'True', 'True', 'True'),
(50,60,1,36,1, 'True', 'True', 'True'),
(51,111,1,36,2, 'True', 'True', 'True'),
(52,60,1,37,1, 'True', 'True', 'True'),
(53,60,1,39,2, 'False', 'True', 'True'),
(54,60,1,40,1, 'True', 'True', 'True'),
(55,60,1,41,2, 'False', 'True', 'True'),
(56,60,1,42,3, 'False', 'True', 'True'),
(57,60,1,43,3, 'False', 'True', 'True'),
(58,60,1,44,1, 'True', 'True', 'True'),
(59,60,1,45,5, 'True', 'True', 'True'),
(60,60,1,46,1, 'True', 'True', 'True'),
(61,60,1,47,3, 'True', 'True', 'True'),
(62,60,1,48,1, 'True', 'True', 'True'),
(63,60,1,48,2, 'True', 'False', 'True'),
(64,120,1,49,2, 'True', 'True', 'True'),
(65,60,1,50,1, 'True', 'True', 'True'),
(66,60,1,51,3, 'True', 'True', 'True'),
(67,60,1,52,4, 'True', 'True', 'True'),
(68,60,1,53,3, 'True', 'True', 'True'),
(69,60,1,54,4, 'True', 'True', 'True'),
(70,60,1,55,3, 'True', 'True', 'True'),
(71,60,1,56,4, 'True', 'True', 'True'),
(72,60,1,60,1, 'True', 'True', 'True'),
(73,60,1,61,3, 'True', 'True', 'True'),
(74,60,1,64,1, 'True', 'True', 'True'),
(75,60,1,65,2, 'True', 'True', 'True'),
(76,60,1,67,1, 'True', 'True', 'True'),
(77,60,1,68,1, 'True', 'True', 'True'),
(78,60,2,69,3, 'True', 'True', 'True'),
(79,85,8,69,4, 'False', 'True', 'True'),
(80,45,3,1,1, 'True', 'True', 'True'),
(81,30,4,1,1, 'True', 'False', 'True'),
(82,60,2,1,2, 'True', 'False', 'True'),
(83,40,3,1,2, 'True', 'False', 'True'),
(84,44,3,2,1, 'True', 'True', 'True'),
(85,22,4,2,1, 'True', 'False', 'True'),
(86,99,1,2,1, 'True', 'False', 'True'),
(87,75,1,1,1, 'True', 'True', 'True');
SET IDENTITY_INSERT dbo.StateStationActivities OFF

SET IDENTITY_INSERT dbo.StateStationActivityMachines ON
insert into dbo.StateStationActivityMachines (Id, IsFixed, StateStationActivity_Id, Machine_Id) values 
('1','True','1','1'),
('2','False','1','2'),
('3','False','1','3'),
('4','True','2','1'),
('5','False','2','2'),
('6','False','2','3'),
('7','True','3','3'),
('8','False','3','4'),
('9','True','4','2'),
('10','False','4','4'),
('11','True','5','9'),
('12','False','5','10'),
('13','False','6','9'),
('14','True','6','10'),
('15','True','7','9'),
('16','True','7','10'),
('17','False','7','11'),
('18','True','8','1'),
('19','True','8','2'),
('20','True','9','1'),
('21','True','9','2'),
('22','False','9','3'),
('23','True','10','2'),
('24','False','10','4'),
('25','True','11','3'),
('26','True','12','2'),
('27','True','13','1'),
('28','False','13','2'),
('29','True','14','2'),
('30','True','15','3'),
('31','True','16','5'),
('32','True','17','6'),
('33','True','18','9'),
('34','True','19','1'),
('35','True','20','3'),
('36','True','21','2'),
('37','True','22','2'),
('38','True','23','2'),
('39','True','24','2'),
('40','True','25','3'),
('41','True','26','3'),
('42','True','27','3'),
('43','True','28','8'),
('44','True','29','8'),
('45','True','30','9'),
('46','True','30','10'),
('47','True','31','9'),
('48','True','31','11'),
('49','True','32','1'),
('50','False','33','1'),
('51','True','33','2'),
('52','False','33','3'),
('53','True','34','3'),
('54','True','35','3'),
('55','True','36','4'),
('56','True','37','4'),
('57','True','38','4'),
('58','True','39','6'),
('59','True','40','2'),
('60','True','41','3'),
('61','True','42','1'),
('62','True','43','8'),
('63','True','44','9'),
('64','True','45','1'),
('65','True','46','8'),
('66','True','47','9'),
('67','True','48','7'),
('68','True','49','6'),
('69','True','50','7'),
('70','True','51','7'),
('71','True','52','6'),
('72','True','53','7'),
('73','True','54','8'),
('74','True','55','3'),
('75','True','56','9'),
('76','True','57','10'),
('77','True','58','4'),
('78','True','59','9'),
('79','True','60','5'),
('80','True','61','11'),
('81','True','62','4'),
('82','True','63','4'),
('83','True','64','5'),
('84','True','65','9'),
('85','True','66','9'),
('86','True','67','6'),
('87','True','68','7'),
('88','True','69','11'),
('89','True','70','1'),
('90','True','71','5'),
('91','True','76','4'),
('92','True','77','2');
SET IDENTITY_INSERT dbo.StateStationActivityMachines OFF

SET IDENTITY_INSERT dbo.Causes ON
insert into dbo.Causes (Id, [Level], Name, Code, Parent_Id, CreatedDate, ModifiedDate, [Status], ModifiedBy) values 
('2','1',N'فنی','0', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('3','1',N'پرسنلی','1', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('4','1',N'سایر','99', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
--فنی
('5','2',N'برق','0', '2', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('6','2',N'دستگاه','1', '2', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('7','2',N'مواد','2', '2', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
--پرسنلی
('8','2',N'خطای اپراتور','0', '3', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('9','2',N'خود اپراتور','1', '3', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
--سایر
('10','2',N'اجتماعی','0', '4', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('11','2',N'سیاسی اقتصادی','1', '4', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('12','2',N'علمی فرهنگی هنری','2', '4', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

--فنی
('13','3',N'قطعی برق','0', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('14','3',N'وصلی برق','1', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('15','3',N'قطع و وصل برق','2', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('16','3',N'نوسانات برق','3', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('17','3',N'برق گرفتگی','4', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('18','3',N'خرابی دستگاه','0', '6', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('19','3',N'گیر کردن دست اپراتور در دستگاه','1', '6', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('20','3',N'روغنکاری','2', '6', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('21','3',N'پنچری','3', '6', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('22','3',N'مواد بد','0', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('23','3',N'ذغال بد','1', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('24','3',N'رفیق بد','2', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('25','3',N'فلان','0', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

--پرسنلی
('26','3',N'کم اشتهایی','0', '8', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('27','3',N'اسهال','1', '8', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('28','3',N'بواسیر','2', '8', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('29','3',N'مشکلات شخصیتی','0', '9', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('30','3',N'تغذیه دوران کودکی','1', '9', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('31','3',N'محل تولد','2', '9', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('32','3',N'فقر','0', '10', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('33','3',N'فحشا','1', '10', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('34','3',N'رانت خواری','10', '11', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('35','3',N'بگم بگم بازی','11', '11', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('36','3',N'تراکم','12', '11', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('37','3',N'پسا پست مدرنسیم','20', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('38','3',N'تنهایی انسان در عصر ماشینی','21', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.Causes OFF

--SET IDENTITY_INSERT dbo.xxxxxx ON
--insert into dbo.Causes (Id, [Level], Name, Code, Parent_Id, CreatedDate, ModifiedDate, [Status], ModifiedBy) values 
--('37','2',N'پسا پست مدرنسیم','20', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
--('38','2',N'تنهایی انسان در عصر ماشینی','21', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
--SET IDENTITY_INSERT dbo.xxxxxx OFF

use SoheilDb
