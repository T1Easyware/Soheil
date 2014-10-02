
USE [SoheilDb]
INSERT INTO AccessRules ([Id],[Code],[Name],[Parent_Id]) VALUES
(1,'0','txtSoheil',null),
	(2,'1','txtUsers',1),
		(3,'11','txtUserAccounts',2),
			(4,'111','txtUsers',3),
			(5,'112','txtPositions',3),
			(6,'113','txtOrgCharts',3),
		(7,'12','txtModules',2),
			(8,'121','txtModules',7),
		(41,'13','txtOrganizationCalendar',2),
			(42,'131','txtWorkProfiles',41),
			(43,'132','txtHolidays',41),
			(44,'133','txtWorkProfilePlan',41),
	(9,'2','txtDefinitions',1),
		(10,'21','txtProducts',9),
			(11,'211','txtProducts',10),
			(12,'212','txtReworks',10),
		(13,'22','txtDiagnosis',9),
			(14,'221','txtDefections',13),
			(15,'222','txtRoots',13),
			(16,'223','txtActionPlans',13),
			(17,'224','txtCauses',13),
		(18,'23','txtFPC',9),
			(19,'231','txtFPC',18),
			(20,'232','txtStations',18),
			(21,'233','txtMachines',18),
			(22,'234','txtActivities',18),
		(23,'24','txtOperators',9),
			(24,'241','txtOperators',23),
			(25,'242','txtGenSkills',23),
			(26,'243','txtSpeSkills',23),
		(27,'25','txtCosts',9),
			(28,'251','txtCosts',27),
			(29,'252','txtPartWarehouses',27),
		(45,'26','txtSetupTimes',9),
		(46,'27','txtSkillCenter',9),
	(30,'3','txtControl',1),
		(31,'31','txtProductPlan',30),
		(32,'32','txtPerformance',30),
		(33,'33','txtIndices',30),
	(34,'4','txtReports',1),
		(35,'41','txtCostReports',34),
		(36,'42','txtActualCostReports',34),
		(37,'43','txtOperationReports',34),
	(38,'5','txtOptions',1),
		(39,'51','txtSettings',37),
		(40,'52','txtHelp',37),
		(47,'53','txtAbout',37),
	(48,'6','txtStorage',1),
		(49,'61','txtWarehouse',48),
		(50,'62','txtWarehouseTransactions',48),
			(55,'621','txtRawMaterialStorage',50),
			(56,'622','txtStockDischarge',50),
		(51,'63','txtRawMaterials',48),
		(52,'64','txtUnits',52),
			(53,'641','txtUnitSets',52),
			(54,'642','txtUnitConversions',52);



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
(54,1,0,'2013-01-01',31),
(55,1,0,'2013-01-01',31),
(56,1,0,'2013-01-01',31);


INSERT INTO Causes([Name],[Code],[Description],[ModifiedDate],[CreatedDate],[Status],[ModifiedBy],[Level]) VALUES
('Causes',0,'','2013-01-01','2013-01-01',1,0,0);



-------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------------------------------
use SoheilDb

SET IDENTITY_INSERT dbo.Warehouses ON
insert into dbo.Warehouses (Id, Code, Name, ModifiedDate, CreatedDate, [Status], ModifiedBy, Location, HasFinalProduct, HasRawMaterial, HasWIP) values 
(1, '1', N'اصلی', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1,'Basement',1,1,1),
(2, '2', N'نیم ساخته', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1,'Basement',0,1,1),
(3, '2', N'انبارنهایی', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1,'Basement',1,1,0);
SET IDENTITY_INSERT dbo.Warehouses OFF




SET IDENTITY_INSERT dbo.Operators ON
insert into dbo.Operators (Id, Name, Code, Score, CreatedDate, ModifiedDate, [Status], ModifiedBy, Age, Sex) values 
(1, N'پژمان مسعودی', '001', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(2, N'زهرا کرمی زاده', '002', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'False'),
(3, N'مهدی علیدوست', '003', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(4, N'سجاد سلطانیان', '004', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True');
SET IDENTITY_INSERT dbo.Operators OFF

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

('22','3',N'مواد نامرغوب 1','0', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('23','3',N'مواد نامرغوب 2','1', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('24','3',N'مواد نامرغوب 3','2', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('25','3',N'فلان','0', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

--پرسنلی
('26','3',N'دلیل 3','0', '8', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('27','3',N'دلیل 3','1', '8', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('28','3',N'دلیل 3','2', '8', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('29','3',N'دلیل نامرغوب 3','0', '9', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('30','3',N'دلیل ','1', '9', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('31','3',N'دلیل 3 تولد','2', '9', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('32','3',N'دلیل 3','0', '10', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('33','3',N'دلیل 3','1', '10', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('34','3',N'دلیل 3','10', '11', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('35','3',N'دلیل 3','11', '11', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('36','3',N'تردلیلاکم','12', '11', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('37','3',N'دلیل 3','20', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('38','3',N'دلیل 3','21', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.Causes OFF

--SET IDENTITY_INSERT dbo.xxxxxx ON
--insert into dbo.Causes (Id, [Level], Name, Code, Parent_Id, CreatedDate, ModifiedDate, [Status], ModifiedBy) values 
--('37','2',N'پسا پست مدرنسیم','20', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
--('38','2',N'تنهایی انسان در عصر ماشینی','21', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
--SET IDENTITY_INSERT dbo.xxxxxx OFF


SET IDENTITY_INSERT dbo.UnitGroups ON
insert into UnitGroups([Id],[ModifiedBy],[Name],[Status]) values
(1,1,N'وزنی',1);

insert into UnitGroups([Id],[ModifiedBy],[Name],[Status]) values
(2,1,N'حجمی',1); 

insert into UnitGroups([Id],[ModifiedBy],[Name],[Status]) values
(3,1,N'سایر',1); 
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

insert into UnitSets([Id],[Code],[Description],[ModifiedBy],[Status],[UnitGroup_Id]) values
(7,'U', N'تعداد',1,1,3); 

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



use SoheilDb
