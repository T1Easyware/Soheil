
USE SoheilDb
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
	(37,'5','txtOptions',1),
		(38,'51','txtSettings',37),
		(39,'52','txtHelp',37),
		(40,'53','txtAbout',37);



ALTER TABLE USERS ADD CONSTRAINT USER_UNIQUE_CODE UNIQUE (CODE);


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
(43,1,0,'2013-01-01',31);


--INSERT INTO Causes([Name],[Code],[Description],[ModifiedDate],[CreatedDate],[Status],[ModifiedBy],[Level]) VALUES
--('Causes',0,'','2013-01-01','2013-01-01',1,0,0);



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
