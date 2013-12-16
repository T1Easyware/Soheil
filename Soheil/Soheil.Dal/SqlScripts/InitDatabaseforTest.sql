﻿use SoheilDb
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
insert into dbo.Defections (Id, Code, Name, IsG2, ModifiedDate, CreatedDate, [Status], ModifiedBy) values 
(1, 'Df1', N'خال خال', 'True', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(2, 'Df2', N'دانه دار', 'True', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(3, 'Df3', N'معیوب', 'False', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(4, 'Df4', N'پارگی', 'False', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(5, 'Df5', N'سوراخدار', 'False', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
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
insert into dbo.ProductReworks (Id, Code, Name, Product_Id, Rework_Id, [Status], ModifiedBy) values 
(1, 'Pr1Main', N'زانویی سمند', 1, NULL, 1, 1),
(2, 'Pr1R1', N'زانویی سمند دوباره کاری 1', 1, 1, 1, 1),
(3, 'Pr1R2', N'زانویی سمند دوباره کاری 2', 1, 2, 1, 1),
(4, 'Pr2Main', N'زانویی L90', 2, NULL, 1, 1),
(5, 'Pr2R1', N'زانویی L90 دوباره کاری 1', 2, 1, 1, 1),
(6, 'Pr2R3', N'زانویی L90 دوباره کاری 3', 2, 3, 1, 1),
(7, 'Pr3Main', N'داکت راست سمند', 3, NULL, 1, 1),
(8, 'Pr4Main', N'داکت چپ سمند', 4, NULL, 1, 1),
(9, 'Zan_pjo', N'زانویی پژو',6, NULL, 1, 1),
(10, 'Zan_pjo', N'زانویی پژو سوراخ',6, 3, 1, 1),
(11, 'RD_prd', N'داکت راست پراید',7, NULL, 1, 1),
(12, 'RD_prd', N'داکت راست پراید نوع1',7, 1, 1, 1),
(13, 'RD_prd', N'داکت راست پراید نوع2',7, 2, 1, 1),
(14, 'RD_pjo', N'داکت راست پژو',8, NULL, 1, 1),
(15, 'LD_prd', N'داکت چپ پراید', 9, NULL, 1, 1),
(16, 'LD_prd', N'داکت چپ پراید خالخالی', 9, 4, 1, 1),
(17, 'LD_pjo', N'داکت چپ پژو',10, NULL, 1, 1),
(18, 'Prod1', N'محصول جدید 1',11, NULL, 1, 1),
(19, 'Prod1', N'محصول جدید 1سفیدک',11, 4, 1, 1),
(20, 'Prod1', N'محصول جدید 1دانه دار',11, 5, 1, 1),
(21, 'Prod2', N'محصول جدید 2',12, NULL, 1, 1),
(22, 'Prod2', N'محصول جدید خراب2',12, 5, 1, 1),
(23, 'Prod3', N'محصول جدید 3',13, NULL, 1, 1),
(24, 'Prod3', N'محصول جدید مشکل دار3',13, 3, 1, 1),
(25, 'Prod4', N'محصول جدید 4',14, NULL, 1, 1),
(26, 'Prod4', N'محصول جدید 4شل',14, 1, 1, 1),
(27, 'Prod4', N'محصول جدید 4سفت',14, 2, 1, 1),
(28, 'Prod4', N'محصول جدید 4کج',14, 3, 1, 1),
(29, 'Prod5', N'محصول جدید 5',15, NULL, 1, 1),
(30, 'Prod5', N'محصول جدید 5خراب',15, 3, 1, 1),
(31, 'Prod5', N'محصول جدید 5دوباره کاری دارد',15, 5, 1, 1),
(32, 'ProdNew1', N'محصول جدید دیگر 1',16, NULL, 1, 1),
(33, 'ProdNew1', N'محصول جدید دیگر 1مشکل دارد',16, 3, 1, 1),
(34, 'ProdNew1', N'محصول جدید دیگر 1بدون پیچ',16, 4, 1, 1),
(35, 'ProdNew2', N'محصول جدید دیگر 2',17, NULL, 1, 1),
(36, 'ProdNew2', N'محصول جدید دیگر 2شل',17, 4, 1, 1),
(37, 'ProdNew2', N'محصول جدید دیگر کج2',17, 5, 1, 1),
(38, 'ProdNew3', N'محصول جدید دیگر 3',18, NULL, 1, 1),
(39, 'ProdNew3', N'محصول جدید دیگر 3شل',18, 1, 1, 1),
(40, 'ProdNew4', N'محصول جدید دیگر 4',19, NULL, 1, 1),
(41, 'ProdNew4', N'محصول جدید دیگر 4شل',19, 3, 1, 1),
(42, 'ProdNew5', N'محصول جدید دیگر 5',20, NULL, 1, 1),
(43, 'ProdOther1', N'باز هم محصول جدید دیگر 1',21, NULL, 1, 1),
(44, 'ProdOther1', N'باز هم محصول جدید دیگر 1خرابی1',21, 4, 1, 1),
(45, 'ProdOther1', N'باز هم محصول جدید دیگر 1خرابی2',21, 5, 1, 1),
(46, 'ProdOther2', N'باز هم محصول جدید دیگر 2',22, NULL, 1, 1),
(47, 'ProdOther2', N'باز هم محصول جدید دیگر 2خرابی1',22, 3, 1, 1),
(48, 'ProdOther2', N'باز هم محصول جدید دیگر 2خرابی2',22, 4, 1, 1),
(49, 'ProdOther2', N'باز هم محصول جدید دیگر 2خرابی3',22, 5, 1, 1),
(50, 'ProdOther3', N'باز هم محصول جدید دیگر 3',23, NULL, 1, 1),
(51, 'ProdOther3', N'باز هم محصول جدید دیگر 3خراب',23, 2, 1, 1),
(52, 'ProdOther5', N'باز هم محصول جدید دیگر 5',25, NULL, 1, 1),
(53, 'ProdOther5', N'باز هم محصول جدید دیگر 5خراب',25, 1, 1, 1),
(54, 'Zan_prd', N'زانویی پراید',5, NULL, 1, 1);
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
insert into dbo.Machines (Id, Code, Name, MachineFamily_Id, ModifiedDate, CreatedDate, [Status], ModifiedBy) values 
(1, 'PE1', N'PEBM01', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(2, 'PE2', N'PEBM02', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(3, 'PE3', N'PEBM03', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(4, 'PE4', N'PEBM04', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(5, 'PE5', N'PEBM05', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(6, 'PVC1', N'PVCBM06', 2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(7, 'PVC2', N'PVCBM07', 2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(8, 'PVC3', N'PVCBM08', 2, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(9, 'Cut1', N'کاتر برقی', 3, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(10, 'Cut2', N'فرز', 3, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
(11, 'PE_', N'خالی', 1, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.Machines OFF

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
insert into dbo.Operators (Id, Name, Code, Score, CreatedDate, ModifiedDate, [Status], ModifiedBy) values 
(1, N'پژمان مسعودی', '001', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(2, N'سید جواد کرمی زاده', '002', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(3, N'مهدی علیدوست', '003', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(4, N'سجاد سلطانیان', '004', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(5, N'هوشنگ هدایت', '005', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(6, N'فریدون هاشمی', '006', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(7, N'سهیل سعادتیان', '007', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(8, N'فرهاد حاج مرادی', '008', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(9, N'پشمان مجعودی', '009', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(10, N'پجمان مسفودی', '011', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(11, N'پچمان مستودی', '021', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(12, N'سید جلاد نرمی زاده', '102', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(13, N'مهلی علیداست', '103', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(14, N'سعاد سلصانیان', '204', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(15, N'هوبنگ هتایت', '015', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(16, N'فرییون هاخمی', '106', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(17, N'سهیم سقدتیان', '237', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(18, N'فرجاد حاج مقادی', '128', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(19, N'فریاد حاج مدادی', '328', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1),
(20, N'فرخاد حاج منادی', '458', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1);
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
insert into dbo.Connectors (Id, StartState_Id, EndState_Id) values 
('1','1','2'),
('2','2','4'),
('3','2','6'),
('4','4','3'),
('5','3','6'),
('6','3','5'),
('7','5','2'),
('16','7','81'),
('17','7','80'),
('18','80','10'),
('19','10','79'),
('20','79','8'),
('21','80','8'),
('22','81','9'),
('23','9','79'),
('39','11','82'),
('40','82','83'),
('41','83','84'),
('42','84','12'),
('43','82','84'),
('65','13','85'),
('66','13','86'),
('67','86','87'),
('68','85','87'),
('69','85','90'),
('70','87','90'),
('71','90','88'),
('72','87','88'),
('73','88','89'),
('74','86','89'),
('75','89','14'),
('84','16','91'),
('85','91','18'),
('86','91','19'),
('87','91','92'),
('88','92','17'),
('89','18','93'),
('90','19','93'),
('91','93','92'),
('92','20','94'),
('93','94','95'),
('94','95','21'),
('95','96','21'),
('102','22','99'),
('103','99','98'),
('104','98','24'),
('105','24','97'),
('106','97','23'),
('107','98','23'),
('108','25','100'),
('109','100','26'),
('117','27','101'),
('118','101','29'),
('119','29','103'),
('120','103','28'),
('121','101','30'),
('122','30','102'),
('123','102','28'),
('124','101','28'),
('130','31','104'),
('131','104','33'),
('132','33','105'),
('133','105','32'),
('134','104','32'),
('135','34','106'),
('136','106','35'),
('137','106','36'),
('138','36','107'),
('139','107','35'),
('140','37','108'),
('141','108','38'),
('142','108','39'),
('143','108','40'),
('144','108','41'),
('145','39','109'),
('146','40','109'),
('147','40','110'),
('148','110','38'),
('149','41','110'),
('150','109','38'),
('153','42','111'),
('154','111','43'),
('155','111','112'),
('156','112','43'),
('157','112','44'),
('158','112','45'),
('159','45','113'),
('160','44','113'),
('161','113','43'),
('162','46','114'),
('163','114','47'),
('164','50','115'),
('165','115','51'),
('166','115','52'),
('167','115','53'),
('168','52','116'),
('169','53','116'),
('170','116','51'),
('171','54','117'),
('172','117','56'),
('173','117','55'),
('174','56','118'),
('175','118','55'),
('176','57','119'),
('177','119','59'),
('178','59','120'),
('179','120','58'),
('180','119','58'),
('181','60','124'),
('182','124','122'),
('183','122','121'),
('184','121','123'),
('185','123','61'),
('186','62','64'),
('187','64','63'),
('188','63','65'),
('189','66','125'),
('190','125','68'),
('191','68','126'),
('192','126','70'),
('193','70','125'),
('194','66','67'),
('195','67','127'),
('196','127','69'),
('197','71','128'),
('198','128','73'),
('199','73','128'),
('200','128','72'),
('201','74','75'),
('202','76','129'),
('203','129','130'),
('204','130','129'),
('205','78','77');
SET IDENTITY_INSERT dbo.Connectors OFF

SET IDENTITY_INSERT dbo.StationMachines ON
insert into dbo.StationMachines (Id, Station_Id, Machine_Id) values 
(1, 1, 1),
(2, 2, 2),
(3, 3, 3),
(4, 4, 4),
(5, 5, 5),
(6, 6, 6),
(7, 7, 7),
(8, 8, 8),
(9, 9, 9),
(10, 9, 10),
(11, 9, 11),
(12, 1, 2),
(13, 1, 3),
(14, 2, 3),
(15, 2, 4);
SET IDENTITY_INSERT dbo.StationMachines OFF

SET IDENTITY_INSERT dbo.StateStations ON
insert into dbo.StateStations (Id, State_Id, Station_Id) values 
(1,2,1),
(2,2,2),
(3,2,9),
(4,3,1),
(5,79,2),
(6,79,3),
(7,80,2),
(8,81,1),
(9,82,2),
(10,83,3),
(11,84,5),
(12,84,6),
(13,82,9),
(14,85,1),
(15,85,2),
(16,86,2),
(17,86,3),
(18,86,8),
(19,86,9),
(20,87,1),
(21,87,3),
(22,88,3),
(23,88,4),
(24,89,4),
(25,89,6),
(26,90,2),
(27,90,3),
(28,91,1),
(29,92,8),
(30,93,9),
(31,94,1),
(32,95,8),
(33,96,9),
(34,97,7),
(35,98,6),
(36,98,7),
(37,99,6),
(38,99,8),
(39,99,7),
(40,100,8),
(41,101,3),
(42,102,9),
(43,103,9),
(44,104,4),
(45,105,9),
(46,106,5),
(47,107,9),
(48,108,4),
(49,108,5),
(50,109,9),
(51,110,9),
(52,111,6),
(53,112,7),
(54,113,9),
(55,114,1),
(56,115,5),
(57,116,9),
(58,117,1),
(59,118,9),
(60,119,1),
(61,120,9),
(62,121,1),
(63,121,2),
(64,122,2),
(65,122,3),
(66,122,1),
(67,124,4),
(68,124,1),
(69,2,6);
SET IDENTITY_INSERT dbo.StateStations OFF

SET IDENTITY_INSERT dbo.StateStationActivities ON
insert into dbo.StateStationActivities (Id, CycleTime, ManHour, StateStation_Id, Activity_Id) values 
(1,60,2,1,1),
(2,90,1,1,2),
(3,60,2,2,1),
(4,85,1.33,2,2),
(5,40,1,3,3),
(6,50,1,3,4),
(7,60,1,3,5),
(8,60,3,4,3),
(9,110,1,4,4),
(10,60,1,5,2),
(11,60,1,6,1),
(12,60,1,7,1),
(13,60,1,8,1),
(14,60,1,9,1),
(15,60,1,10,2),
(16,60,1,11,1),
(17,60,1,12,2),
(18,60,1,13,1),
(19,60,1,14,1),
(20,60,1,21,2),
(21,60,1,15,2),
(22,60,1,16,1),
(23,60,1,16,2),
(24,60,1,16,5),
(25,60,1,17,2),
(26,60,1,17,3),
(27,60,1,17,4),
(28,60,1,18,4),
(29,60,1,18,2),
(30,60,1,19,3),
(31,60,1,19,1),
(32,60,1,20,1),
(33,60,1,20,2),
(34,60,1,22,3),
(35,60,1,22,1),
(36,60,1,23,4),
(37,60,1,23,1),
(38,60,1,24,3),
(39,60,1,25,4),
(40,60,1,26,3),
(41,60,1,27,4),
(42,60,1,28,1),
(43,60,1,29,1),
(44,60,1,30,3),
(45,60,1,31,1),
(46,60,1,32,1),
(47,60,1,33,3),
(48,333,1,34,1),
(49,222,1,35,1),
(50,60,1,36,1),
(51,111,1,36,2),
(52,60,1,37,1),
(53,60,1,39,2),
(54,60,1,40,1),
(55,60,1,41,2),
(56,60,1,42,3),
(57,60,1,43,3),
(58,60,1,44,1),
(59,60,1,45,5),
(60,60,1,46,1),
(61,60,1,47,3),
(62,60,1,48,1),
(63,60,1,48,2),
(64,120,1,49,2),
(65,60,1,50,1),
(66,60,1,51,3),
(67,60,1,52,4),
(68,60,1,53,3),
(69,60,1,54,4),
(70,60,1,55,3),
(71,60,1,56,4),
(72,60,1,60,1),
(73,60,1,61,3),
(74,60,1,64,1),
(75,60,1,65,2),
(76,60,1,67,1),
(77,60,1,68,1),
(78,60,2,69,3),
(79,85,1.33,69,4);
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
('1','0',N'فنی','0', NULL, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('2','0',N'پرسنلی','1', NULL, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('3','0',N'سایر','99', NULL, {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
--فنی
('4','1',N'برق','0', '1', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('5','1',N'دستگاه','1', '1', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('6','1',N'مواد','2', '1', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('7','1',N'سایر','99', '1', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
--پرسنلی
('8','1',N'خطای اپراتور','0', '2', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('9','1',N'خود اپراتور','1', '2', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
--سایر
('10','1',N'اجتماعی','0', '3', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('11','1',N'سیاسی اقتصادی','1', '3', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('12','1',N'علمی فرهنگی هنری','2', '3', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

--فنی
('13','2',N'قطعی برق','0', '4', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('14','2',N'وصلی برق','1', '4', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('15','2',N'قطع و وصل برق','2', '4', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('16','2',N'نوسانات برق','3', '4', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('17','2',N'برق گرفتگی','4', '4', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('18','2',N'خرابی دستگاه','0', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('19','2',N'گیر کردن دست اپراتور در دستگاه','1', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('20','2',N'روغنکاری','2', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('21','2',N'پنچری','3', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('22','2',N'مواد بد','0', '6', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('23','2',N'ذغال بد','1', '6', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('24','2',N'رفیق بد','2', '6', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('25','2',N'فلان','0', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

--پرسنلی
('26','2',N'کم اشتهایی','0', '8', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('27','2',N'اسهال','1', '8', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('28','2',N'بواسیر','2', '8', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('29','2',N'مشکلات شخصیتی','0', '9', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('30','2',N'تغذیه دوران کودکی','1', '9', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('31','2',N'محل تولد','2', '9', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('32','2',N'فقر','0', '10', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('33','2',N'فحشا','1', '10', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('34','2',N'رانت خواری','10', '11', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('35','2',N'بگم بگم بازی','11', '11', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('36','2',N'تراکم','12', '11', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('37','2',N'پسا پست مدرنسیم','20', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('38','2',N'تنهایی انسان در عصر ماشینی','21', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.StateStationActivityMachines OFF

--SET IDENTITY_INSERT dbo.xxxxxx ON
--insert into dbo.Causes (Id, [Level], Name, Code, Parent_Id, CreatedDate, ModifiedDate, [Status], ModifiedBy) values 
--('37','2',N'پسا پست مدرنسیم','20', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
--('38','2',N'تنهایی انسان در عصر ماشینی','21', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
--SET IDENTITY_INSERT dbo.xxxxxx OFF

use SoheilDb
