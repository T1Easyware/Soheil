
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 08/08/2014 00:04:24
-- Generated from EDMX file: D:\Repo\Soheil\Soheil.Dal\SoheilEdm.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [SoheilDb];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_ActivityGroupActivity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Activities] DROP CONSTRAINT [FK_ActivityGroupActivity];
GO
IF OBJECT_ID(N'[dbo].[FK_MachineFamilyMachine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Machines] DROP CONSTRAINT [FK_MachineFamilyMachine];
GO
IF OBJECT_ID(N'[dbo].[FK_StateStateStation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StateStations] DROP CONSTRAINT [FK_StateStateStation];
GO
IF OBJECT_ID(N'[dbo].[FK_StateConnector]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Connectors] DROP CONSTRAINT [FK_StateConnector];
GO
IF OBJECT_ID(N'[dbo].[FK_StateConnector1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Connectors] DROP CONSTRAINT [FK_StateConnector1];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductProductDefection]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductDefections] DROP CONSTRAINT [FK_ProductProductDefection];
GO
IF OBJECT_ID(N'[dbo].[FK_DefectionProductDefection]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductDefections] DROP CONSTRAINT [FK_DefectionProductDefection];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUser_UserGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[User_Positions] DROP CONSTRAINT [FK_UserUser_UserGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_UserGroupUser_UserGroup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[User_Positions] DROP CONSTRAINT [FK_UserGroupUser_UserGroup];
GO
IF OBJECT_ID(N'[dbo].[FK_UserGroupUserGroup_AccessRule]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Position_AccessRules] DROP CONSTRAINT [FK_UserGroupUserGroup_AccessRule];
GO
IF OBJECT_ID(N'[dbo].[FK_UserUser_AccessRule]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[User_AccessRules] DROP CONSTRAINT [FK_UserUser_AccessRule];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductGroupProduct]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Products] DROP CONSTRAINT [FK_ProductGroupProduct];
GO
IF OBJECT_ID(N'[dbo].[FK_PositionOrganizationChart_Position]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrganizationChart_Position] DROP CONSTRAINT [FK_PositionOrganizationChart_Position];
GO
IF OBJECT_ID(N'[dbo].[FK_OrganizationChartOrganizationChart_Position]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrganizationChart_Position] DROP CONSTRAINT [FK_OrganizationChartOrganizationChart_Position];
GO
IF OBJECT_ID(N'[dbo].[FK_AccessRuleUser_AccessRule]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[User_AccessRules] DROP CONSTRAINT [FK_AccessRuleUser_AccessRule];
GO
IF OBJECT_ID(N'[dbo].[FK_AccessRulePosition_AccessRule]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Position_AccessRules] DROP CONSTRAINT [FK_AccessRulePosition_AccessRule];
GO
IF OBJECT_ID(N'[dbo].[FK_OrganizationChart_PositionOrganizationChart_Position]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrganizationChart_Position] DROP CONSTRAINT [FK_OrganizationChart_PositionOrganizationChart_Position];
GO
IF OBJECT_ID(N'[dbo].[FK_AccessRuleAccessRule]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AccessRules] DROP CONSTRAINT [FK_AccessRuleAccessRule];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductProduct]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Products] DROP CONSTRAINT [FK_ProductProduct];
GO
IF OBJECT_ID(N'[dbo].[FK_RootProductDefection_Root]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FishboneNodes] DROP CONSTRAINT [FK_RootProductDefection_Root];
GO
IF OBJECT_ID(N'[dbo].[FK_DefectionReportProductDefection_Root]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FishboneNodes] DROP CONSTRAINT [FK_DefectionReportProductDefection_Root];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductDefection_RootProductDefection_Root]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FishboneNodes] DROP CONSTRAINT [FK_ProductDefection_RootProductDefection_Root];
GO
IF OBJECT_ID(N'[dbo].[FK_ActionPlanProductDefectionRoot_ActionPlan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FishboneNode_ActionPlan] DROP CONSTRAINT [FK_ActionPlanProductDefectionRoot_ActionPlan];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductDefection_RootProductDefectionRoot_ActionPlan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FishboneNode_ActionPlan] DROP CONSTRAINT [FK_ProductDefection_RootProductDefectionRoot_ActionPlan];
GO
IF OBJECT_ID(N'[dbo].[FK_MachineStationMachine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StationMachines] DROP CONSTRAINT [FK_MachineStationMachine];
GO
IF OBJECT_ID(N'[dbo].[FK_FPCState]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[States] DROP CONSTRAINT [FK_FPCState];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductProductRework]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductReworks] DROP CONSTRAINT [FK_ProductProductRework];
GO
IF OBJECT_ID(N'[dbo].[FK_ReworkProductRework]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductReworks] DROP CONSTRAINT [FK_ReworkProductRework];
GO
IF OBJECT_ID(N'[dbo].[FK_ActionPlanDefectionReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DefectionReports] DROP CONSTRAINT [FK_ActionPlanDefectionReport];
GO
IF OBJECT_ID(N'[dbo].[FK_StateStationStateStationActivity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StateStationActivities] DROP CONSTRAINT [FK_StateStationStateStationActivity];
GO
IF OBJECT_ID(N'[dbo].[FK_StateStationActivityStateStationActivityMachine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StateStationActivityMachines] DROP CONSTRAINT [FK_StateStationActivityStateStationActivityMachine];
GO
IF OBJECT_ID(N'[dbo].[FK_MachineStateStationActivityMachine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StateStationActivityMachines] DROP CONSTRAINT [FK_MachineStateStationActivityMachine];
GO
IF OBJECT_ID(N'[dbo].[FK_StateStationActivityMachineSelectedMachine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SelectedMachines] DROP CONSTRAINT [FK_StateStationActivityMachineSelectedMachine];
GO
IF OBJECT_ID(N'[dbo].[FK_ProcessProcessOperator]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProcessOperators] DROP CONSTRAINT [FK_ProcessProcessOperator];
GO
IF OBJECT_ID(N'[dbo].[FK_ProcessSelectedMachine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SelectedMachines] DROP CONSTRAINT [FK_ProcessSelectedMachine];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskProcess]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Process] DROP CONSTRAINT [FK_TaskProcess];
GO
IF OBJECT_ID(N'[dbo].[FK_OperatorProcessOperator]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProcessOperators] DROP CONSTRAINT [FK_OperatorProcessOperator];
GO
IF OBJECT_ID(N'[dbo].[FK_OperatorOperatorStoppageReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Operator_StoppageReports] DROP CONSTRAINT [FK_OperatorOperatorStoppageReport];
GO
IF OBJECT_ID(N'[dbo].[FK_StoppageReportOperatorStoppageReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Operator_StoppageReports] DROP CONSTRAINT [FK_StoppageReportOperatorStoppageReport];
GO
IF OBJECT_ID(N'[dbo].[FK_ProcessReportStoppageReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StoppageReports] DROP CONSTRAINT [FK_ProcessReportStoppageReport];
GO
IF OBJECT_ID(N'[dbo].[FK_TaskTaskReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TaskReports] DROP CONSTRAINT [FK_TaskTaskReport];
GO
IF OBJECT_ID(N'[dbo].[FK_ProcessReportDefectionReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DefectionReports] DROP CONSTRAINT [FK_ProcessReportDefectionReport];
GO
IF OBJECT_ID(N'[dbo].[FK_DefectionReportOperatorDefectionReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OperatorDefectionReports] DROP CONSTRAINT [FK_DefectionReportOperatorDefectionReport];
GO
IF OBJECT_ID(N'[dbo].[FK_MachineCost]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Costs] DROP CONSTRAINT [FK_MachineCost];
GO
IF OBJECT_ID(N'[dbo].[FK_CostCenterCost]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Costs] DROP CONSTRAINT [FK_CostCenterCost];
GO
IF OBJECT_ID(N'[dbo].[FK_PartsWarehouseGroupPartsWarehouse]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PartWarehouses] DROP CONSTRAINT [FK_PartsWarehouseGroupPartsWarehouse];
GO
IF OBJECT_ID(N'[dbo].[FK_PartsWarehouseCost]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Costs] DROP CONSTRAINT [FK_PartsWarehouseCost];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductDefectionRoot]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Roots] DROP CONSTRAINT [FK_ProductDefectionRoot];
GO
IF OBJECT_ID(N'[dbo].[FK_CauseStoppageReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StoppageReports] DROP CONSTRAINT [FK_CauseStoppageReport];
GO
IF OBJECT_ID(N'[dbo].[FK_CauseCause]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Causes] DROP CONSTRAINT [FK_CauseCause];
GO
IF OBJECT_ID(N'[dbo].[FK_StationChangeover]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Changeovers] DROP CONSTRAINT [FK_StationChangeover];
GO
IF OBJECT_ID(N'[dbo].[FK_StationWarmup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Warmups] DROP CONSTRAINT [FK_StationWarmup];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductReworkChangeover]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Changeovers] DROP CONSTRAINT [FK_ProductReworkChangeover];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductReworkWarmup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Warmups] DROP CONSTRAINT [FK_ProductReworkWarmup];
GO
IF OBJECT_ID(N'[dbo].[FK_StationMachinePM]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NonProductiveTasks_PM] DROP CONSTRAINT [FK_StationMachinePM];
GO
IF OBJECT_ID(N'[dbo].[FK_OperatorEducatingOperator]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EducatingOperators] DROP CONSTRAINT [FK_OperatorEducatingOperator];
GO
IF OBJECT_ID(N'[dbo].[FK_EducationEducatingOperator]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EducatingOperators] DROP CONSTRAINT [FK_EducationEducatingOperator];
GO
IF OBJECT_ID(N'[dbo].[FK_ChangeoverSetup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NonProductiveTasks_Setup] DROP CONSTRAINT [FK_ChangeoverSetup];
GO
IF OBJECT_ID(N'[dbo].[FK_WarmupSetup]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NonProductiveTasks_Setup] DROP CONSTRAINT [FK_WarmupSetup];
GO
IF OBJECT_ID(N'[dbo].[FK_NonProductiveTaskReportNonProductiveTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NonProductiveTaskReports] DROP CONSTRAINT [FK_NonProductiveTaskReportNonProductiveTask];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductReworkJob]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Jobs] DROP CONSTRAINT [FK_ProductReworkJob];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductFPC]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[FPCs] DROP CONSTRAINT [FK_ProductFPC];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductReworkState]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[States] DROP CONSTRAINT [FK_ProductReworkState];
GO
IF OBJECT_ID(N'[dbo].[FK_StationStationMachine]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StationMachines] DROP CONSTRAINT [FK_StationStationMachine];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityCost]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Costs] DROP CONSTRAINT [FK_ActivityCost];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductReworkChangeover1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Changeovers] DROP CONSTRAINT [FK_ProductReworkChangeover1];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkShiftWorkBreak]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkBreaks] DROP CONSTRAINT [FK_WorkShiftWorkBreak];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkDayWorkShift]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkShifts] DROP CONSTRAINT [FK_WorkDayWorkShift];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkProfileWorkDay]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkDays] DROP CONSTRAINT [FK_WorkProfileWorkDay];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkProfileWorkProfilePlan]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkProfilePlans] DROP CONSTRAINT [FK_WorkProfileWorkProfilePlan];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkProfileWorkShiftPrototype]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkShiftPrototypes] DROP CONSTRAINT [FK_WorkProfileWorkShiftPrototype];
GO
IF OBJECT_ID(N'[dbo].[FK_WorkShiftPrototypeWorkShift]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[WorkShifts] DROP CONSTRAINT [FK_WorkShiftPrototypeWorkShift];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityStateStationActivity]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StateStationActivities] DROP CONSTRAINT [FK_ActivityStateStationActivity];
GO
IF OBJECT_ID(N'[dbo].[FK_StateStationActivityProcess]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Process] DROP CONSTRAINT [FK_StateStationActivityProcess];
GO
IF OBJECT_ID(N'[dbo].[FK_OperatorCost]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Costs] DROP CONSTRAINT [FK_OperatorCost];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductDefectionDefectionReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[DefectionReports] DROP CONSTRAINT [FK_ProductDefectionDefectionReport];
GO
IF OBJECT_ID(N'[dbo].[FK_OperatorPersonalSkill]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PersonalSkills] DROP CONSTRAINT [FK_OperatorPersonalSkill];
GO
IF OBJECT_ID(N'[dbo].[FK_OperatorOperatorDefectionReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OperatorDefectionReports] DROP CONSTRAINT [FK_OperatorOperatorDefectionReport];
GO
IF OBJECT_ID(N'[dbo].[FK_FPCJob]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Jobs] DROP CONSTRAINT [FK_FPCJob];
GO
IF OBJECT_ID(N'[dbo].[FK_BlockTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Tasks] DROP CONSTRAINT [FK_BlockTask];
GO
IF OBJECT_ID(N'[dbo].[FK_JobBlock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Blocks] DROP CONSTRAINT [FK_JobBlock];
GO
IF OBJECT_ID(N'[dbo].[FK_StationCost]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Costs] DROP CONSTRAINT [FK_StationCost];
GO
IF OBJECT_ID(N'[dbo].[FK_StationTest]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NonProductiveTasks_Test] DROP CONSTRAINT [FK_StationTest];
GO
IF OBJECT_ID(N'[dbo].[FK_ProcessProcessReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProcessReports] DROP CONSTRAINT [FK_ProcessProcessReport];
GO
IF OBJECT_ID(N'[dbo].[FK_StationStateStation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[StateStations] DROP CONSTRAINT [FK_StationStateStation];
GO
IF OBJECT_ID(N'[dbo].[FK_EducationBlock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Blocks] DROP CONSTRAINT [FK_EducationBlock];
GO
IF OBJECT_ID(N'[dbo].[FK_StateStationBlock]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Blocks] DROP CONSTRAINT [FK_StateStationBlock];
GO
IF OBJECT_ID(N'[dbo].[FK_OperatorActivitySkill]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivitySkills] DROP CONSTRAINT [FK_OperatorActivitySkill];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivityActivitySkill]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ActivitySkills] DROP CONSTRAINT [FK_ActivityActivitySkill];
GO
IF OBJECT_ID(N'[dbo].[FK_ProductReworkProductActivitySkill]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductActivitySkills] DROP CONSTRAINT [FK_ProductReworkProductActivitySkill];
GO
IF OBJECT_ID(N'[dbo].[FK_ActivitySkillProductActivitySkill]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ProductActivitySkills] DROP CONSTRAINT [FK_ActivitySkillProductActivitySkill];
GO
IF OBJECT_ID(N'[dbo].[FK_ProcessReportOperatorProcessReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OperatorProcessReports] DROP CONSTRAINT [FK_ProcessReportOperatorProcessReport];
GO
IF OBJECT_ID(N'[dbo].[FK_ProcessOperatorOperatorProcessReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OperatorProcessReports] DROP CONSTRAINT [FK_ProcessOperatorOperatorProcessReport];
GO
IF OBJECT_ID(N'[dbo].[FK_StateStationActivityExternalConnector]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExternalConnectors] DROP CONSTRAINT [FK_StateStationActivityExternalConnector];
GO
IF OBJECT_ID(N'[dbo].[FK_StateStationActivityExternalConnector1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExternalConnectors] DROP CONSTRAINT [FK_StateStationActivityExternalConnector1];
GO
IF OBJECT_ID(N'[dbo].[FK_PartMachinePart]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MachineParts] DROP CONSTRAINT [FK_PartMachinePart];
GO
IF OBJECT_ID(N'[dbo].[FK_MaintenanceMachinePartMaintenance]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MachinePartMaintenances] DROP CONSTRAINT [FK_MaintenanceMachinePartMaintenance];
GO
IF OBJECT_ID(N'[dbo].[FK_MachinePartMachinePartMaintenance]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MachinePartMaintenances] DROP CONSTRAINT [FK_MachinePartMachinePartMaintenance];
GO
IF OBJECT_ID(N'[dbo].[FK_MachinePartMaintenanceMaintenanceReport]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MaintenanceReports] DROP CONSTRAINT [FK_MachinePartMaintenanceMaintenanceReport];
GO
IF OBJECT_ID(N'[dbo].[FK_MachineMachinePart]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MachineParts] DROP CONSTRAINT [FK_MachineMachinePart];
GO
IF OBJECT_ID(N'[dbo].[FK_StoppageReportRepair]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Repairs] DROP CONSTRAINT [FK_StoppageReportRepair];
GO
IF OBJECT_ID(N'[dbo].[FK_MachinePartRepair]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Repairs] DROP CONSTRAINT [FK_MachinePartRepair];
GO
IF OBJECT_ID(N'[dbo].[FK_PM_inherits_NonProductiveTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NonProductiveTasks_PM] DROP CONSTRAINT [FK_PM_inherits_NonProductiveTask];
GO
IF OBJECT_ID(N'[dbo].[FK_Education_inherits_NonProductiveTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NonProductiveTasks_Education] DROP CONSTRAINT [FK_Education_inherits_NonProductiveTask];
GO
IF OBJECT_ID(N'[dbo].[FK_Setup_inherits_NonProductiveTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NonProductiveTasks_Setup] DROP CONSTRAINT [FK_Setup_inherits_NonProductiveTask];
GO
IF OBJECT_ID(N'[dbo].[FK_Test_inherits_NonProductiveTask]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[NonProductiveTasks_Test] DROP CONSTRAINT [FK_Test_inherits_NonProductiveTask];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Products]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Products];
GO
IF OBJECT_ID(N'[dbo].[FPCs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FPCs];
GO
IF OBJECT_ID(N'[dbo].[ProductDefections]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductDefections];
GO
IF OBJECT_ID(N'[dbo].[Defections]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Defections];
GO
IF OBJECT_ID(N'[dbo].[Connectors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Connectors];
GO
IF OBJECT_ID(N'[dbo].[Causes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Causes];
GO
IF OBJECT_ID(N'[dbo].[DefectionReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DefectionReports];
GO
IF OBJECT_ID(N'[dbo].[Stations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Stations];
GO
IF OBJECT_ID(N'[dbo].[Tasks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Tasks];
GO
IF OBJECT_ID(N'[dbo].[Activities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Activities];
GO
IF OBJECT_ID(N'[dbo].[ActivityGroups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivityGroups];
GO
IF OBJECT_ID(N'[dbo].[ProductActivitySkills]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductActivitySkills];
GO
IF OBJECT_ID(N'[dbo].[Operators]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Operators];
GO
IF OBJECT_ID(N'[dbo].[Machines]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Machines];
GO
IF OBJECT_ID(N'[dbo].[MachineFamilies]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MachineFamilies];
GO
IF OBJECT_ID(N'[dbo].[States]', 'U') IS NOT NULL
    DROP TABLE [dbo].[States];
GO
IF OBJECT_ID(N'[dbo].[StateStations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StateStations];
GO
IF OBJECT_ID(N'[dbo].[Process]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Process];
GO
IF OBJECT_ID(N'[dbo].[ProcessOperators]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProcessOperators];
GO
IF OBJECT_ID(N'[dbo].[ProcessReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProcessReports];
GO
IF OBJECT_ID(N'[dbo].[TaskReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TaskReports];
GO
IF OBJECT_ID(N'[dbo].[StoppageReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StoppageReports];
GO
IF OBJECT_ID(N'[dbo].[Operator_StoppageReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Operator_StoppageReports];
GO
IF OBJECT_ID(N'[dbo].[OperatorDefectionReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OperatorDefectionReports];
GO
IF OBJECT_ID(N'[dbo].[Roots]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Roots];
GO
IF OBJECT_ID(N'[dbo].[Reworks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Reworks];
GO
IF OBJECT_ID(N'[dbo].[ProductReworks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductReworks];
GO
IF OBJECT_ID(N'[dbo].[PersonalSkills]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PersonalSkills];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[AccessRules]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AccessRules];
GO
IF OBJECT_ID(N'[dbo].[User_AccessRules]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User_AccessRules];
GO
IF OBJECT_ID(N'[dbo].[Positions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Positions];
GO
IF OBJECT_ID(N'[dbo].[User_Positions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User_Positions];
GO
IF OBJECT_ID(N'[dbo].[Position_AccessRules]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Position_AccessRules];
GO
IF OBJECT_ID(N'[dbo].[ProductGroups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ProductGroups];
GO
IF OBJECT_ID(N'[dbo].[StateStationActivities]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StateStationActivities];
GO
IF OBJECT_ID(N'[dbo].[StateStationActivityMachines]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StateStationActivityMachines];
GO
IF OBJECT_ID(N'[dbo].[OrganizationCharts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrganizationCharts];
GO
IF OBJECT_ID(N'[dbo].[OrganizationChart_Position]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OrganizationChart_Position];
GO
IF OBJECT_ID(N'[dbo].[FishboneNodes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FishboneNodes];
GO
IF OBJECT_ID(N'[dbo].[ActionPlans]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActionPlans];
GO
IF OBJECT_ID(N'[dbo].[FishboneNode_ActionPlan]', 'U') IS NOT NULL
    DROP TABLE [dbo].[FishboneNode_ActionPlan];
GO
IF OBJECT_ID(N'[dbo].[StationMachines]', 'U') IS NOT NULL
    DROP TABLE [dbo].[StationMachines];
GO
IF OBJECT_ID(N'[dbo].[SelectedMachines]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SelectedMachines];
GO
IF OBJECT_ID(N'[dbo].[Costs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Costs];
GO
IF OBJECT_ID(N'[dbo].[CostCenters]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CostCenters];
GO
IF OBJECT_ID(N'[dbo].[PartWarehouses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PartWarehouses];
GO
IF OBJECT_ID(N'[dbo].[PartWarehouseGroups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PartWarehouseGroups];
GO
IF OBJECT_ID(N'[dbo].[Warmups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Warmups];
GO
IF OBJECT_ID(N'[dbo].[Changeovers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Changeovers];
GO
IF OBJECT_ID(N'[dbo].[NonProductiveTasks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NonProductiveTasks];
GO
IF OBJECT_ID(N'[dbo].[EducatingOperators]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EducatingOperators];
GO
IF OBJECT_ID(N'[dbo].[NonProductiveTaskReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NonProductiveTaskReports];
GO
IF OBJECT_ID(N'[dbo].[Jobs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Jobs];
GO
IF OBJECT_ID(N'[dbo].[ObjectiveFunctions]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ObjectiveFunctions];
GO
IF OBJECT_ID(N'[dbo].[WorkProfiles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkProfiles];
GO
IF OBJECT_ID(N'[dbo].[WorkDays]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkDays];
GO
IF OBJECT_ID(N'[dbo].[WorkShifts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkShifts];
GO
IF OBJECT_ID(N'[dbo].[WorkBreaks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkBreaks];
GO
IF OBJECT_ID(N'[dbo].[WorkProfilePlans]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkProfilePlans];
GO
IF OBJECT_ID(N'[dbo].[Holidays]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Holidays];
GO
IF OBJECT_ID(N'[dbo].[WorkShiftPrototypes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[WorkShiftPrototypes];
GO
IF OBJECT_ID(N'[dbo].[OperatorProcessReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[OperatorProcessReports];
GO
IF OBJECT_ID(N'[dbo].[Blocks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Blocks];
GO
IF OBJECT_ID(N'[dbo].[ActivitySkills]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ActivitySkills];
GO
IF OBJECT_ID(N'[dbo].[ExternalConnectors]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExternalConnectors];
GO
IF OBJECT_ID(N'[dbo].[Parts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parts];
GO
IF OBJECT_ID(N'[dbo].[MachineParts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MachineParts];
GO
IF OBJECT_ID(N'[dbo].[Maintenances]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Maintenances];
GO
IF OBJECT_ID(N'[dbo].[MachinePartMaintenances]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MachinePartMaintenances];
GO
IF OBJECT_ID(N'[dbo].[MaintenanceReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MaintenanceReports];
GO
IF OBJECT_ID(N'[dbo].[Repairs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Repairs];
GO
IF OBJECT_ID(N'[dbo].[NonProductiveTasks_PM]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NonProductiveTasks_PM];
GO
IF OBJECT_ID(N'[dbo].[NonProductiveTasks_Education]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NonProductiveTasks_Education];
GO
IF OBJECT_ID(N'[dbo].[NonProductiveTasks_Setup]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NonProductiveTasks_Setup];
GO
IF OBJECT_ID(N'[dbo].[NonProductiveTasks_Test]', 'U') IS NOT NULL
    DROP TABLE [dbo].[NonProductiveTasks_Test];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Products'
CREATE TABLE [dbo].[Products] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ColorNumber] int  NOT NULL,
    [AltColorNumber] int  NOT NULL,
    [ProductGroup_Id] int  NOT NULL,
    [Parent_Id] int  NULL
);
GO

-- Creating table 'FPCs'
CREATE TABLE [dbo].[FPCs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [IsDefault] bit  NOT NULL,
    [Product_Id] int  NOT NULL
);
GO

-- Creating table 'ProductDefections'
CREATE TABLE [dbo].[ProductDefections] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Product_Id] int  NOT NULL,
    [Defection_Id] int  NOT NULL
);
GO

-- Creating table 'Defections'
CREATE TABLE [dbo].[Defections] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL
);
GO

-- Creating table 'Connectors'
CREATE TABLE [dbo].[Connectors] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [HasBuffer] bit  NOT NULL,
    [StartState_Id] int  NOT NULL,
    [EndState_Id] int  NOT NULL
);
GO

-- Creating table 'Causes'
CREATE TABLE [dbo].[Causes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Code] tinyint  NOT NULL,
    [Description] nvarchar(1000)  NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [Level] tinyint  NOT NULL,
    [Parent_Id] int  NULL
);
GO

-- Creating table 'DefectionReports'
CREATE TABLE [dbo].[DefectionReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NULL,
    [LostCount] int  NOT NULL,
    [LostTime] int  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [AffectsTaskReport] bit  NOT NULL,
    [IsG2] bit  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [ActionPlan_Id] int  NULL,
    [ProcessReport_Id] int  NOT NULL,
    [ProductDefection_Id] int  NOT NULL
);
GO

-- Creating table 'Stations'
CREATE TABLE [dbo].[Stations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [Index] int  NOT NULL
);
GO

-- Creating table 'Tasks'
CREATE TABLE [dbo].[Tasks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DurationSeconds] int  NOT NULL,
    [StartDateTime] datetime  NOT NULL,
    [TaskTargetPoint] int  NOT NULL,
    [EndDateTime] datetime  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [Code] nvarchar(max)  NULL,
    [Block_Id] int  NOT NULL
);
GO

-- Creating table 'Activities'
CREATE TABLE [dbo].[Activities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ActivityGroup_Id] int  NOT NULL
);
GO

-- Creating table 'ActivityGroups'
CREATE TABLE [dbo].[ActivityGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL
);
GO

-- Creating table 'ProductActivitySkills'
CREATE TABLE [dbo].[ProductActivitySkills] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IluoNr] tinyint  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ProductRework_Id] int  NOT NULL,
    [ActivitySkill_Id] int  NOT NULL
);
GO

-- Creating table 'Operators'
CREATE TABLE [dbo].[Operators] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Score] real  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [Sex] bit  NOT NULL,
    [Age] int  NOT NULL
);
GO

-- Creating table 'Machines'
CREATE TABLE [dbo].[Machines] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [HasOEE] bit  NOT NULL,
    [MachineFamily_Id] int  NOT NULL
);
GO

-- Creating table 'MachineFamilies'
CREATE TABLE [dbo].[MachineFamilies] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [Status] tinyint  NOT NULL
);
GO

-- Creating table 'States'
CREATE TABLE [dbo].[States] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [StateTypeNr] tinyint  NOT NULL,
    [X] real  NOT NULL,
    [Y] real  NOT NULL,
    [FPC_Id] int  NOT NULL,
    [OnProductRework_Id] int  NULL
);
GO

-- Creating table 'StateStations'
CREATE TABLE [dbo].[StateStations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IsDefault] bit  NOT NULL,
    [State_Id] int  NOT NULL,
    [Station_Id] int  NOT NULL
);
GO

-- Creating table 'Process'
CREATE TABLE [dbo].[Process] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(60)  NULL,
    [TargetCount] int  NOT NULL,
    [StartDateTime] datetime  NOT NULL,
    [EndDateTime] datetime  NOT NULL,
    [DurationSeconds] int  NOT NULL,
    [Task_Id] int  NOT NULL,
    [StateStationActivity_Id] int  NOT NULL
);
GO

-- Creating table 'ProcessOperators'
CREATE TABLE [dbo].[ProcessOperators] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(60)  NULL,
    [RoleNr] tinyint  NOT NULL,
    [Process_Id] int  NOT NULL,
    [Operator_Id] int  NOT NULL
);
GO

-- Creating table 'ProcessReports'
CREATE TABLE [dbo].[ProcessReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(60)  NULL,
    [ProducedG1] int  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ProcessReportTargetPoint] int  NOT NULL,
    [EndDateTime] datetime  NOT NULL,
    [StartDateTime] datetime  NOT NULL,
    [DurationSeconds] int  NOT NULL,
    [Process_Id] int  NOT NULL
);
GO

-- Creating table 'TaskReports'
CREATE TABLE [dbo].[TaskReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(60)  NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ReportStartDateTime] datetime  NOT NULL,
    [ReportDurationSeconds] int  NOT NULL,
    [TaskProducedG1] int  NOT NULL,
    [ReportEndDateTime] datetime  NOT NULL,
    [TaskReportTargetPoint] int  NOT NULL,
    [Task_Id] int  NOT NULL
);
GO

-- Creating table 'StoppageReports'
CREATE TABLE [dbo].[StoppageReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(60)  NULL,
    [LostCount] int  NOT NULL,
    [LostTime] int  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [AffectsTaskReport] bit  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [ProcessReport_Id] int  NOT NULL,
    [Cause_Id] int  NULL
);
GO

-- Creating table 'Operator_StoppageReports'
CREATE TABLE [dbo].[Operator_StoppageReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(60)  NULL,
    [ModifiedBy] int  NOT NULL,
    [Operator_Id] int  NOT NULL,
    [StoppageReport_Id] int  NOT NULL
);
GO

-- Creating table 'OperatorDefectionReports'
CREATE TABLE [dbo].[OperatorDefectionReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(60)  NULL,
    [ModifiedBy] int  NOT NULL,
    [DefectionReport_Id] int  NOT NULL,
    [Operator_Id] int  NOT NULL
);
GO

-- Creating table 'Roots'
CREATE TABLE [dbo].[Roots] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ProductDefection_Id] int  NULL
);
GO

-- Creating table 'Reworks'
CREATE TABLE [dbo].[Reworks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL
);
GO

-- Creating table 'ProductReworks'
CREATE TABLE [dbo].[ProductReworks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [Product_Id] int  NOT NULL,
    [Rework_Id] int  NULL
);
GO

-- Creating table 'PersonalSkills'
CREATE TABLE [dbo].[PersonalSkills] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Education] nvarchar(max)  NOT NULL,
    [Experience] int  NOT NULL,
    [ReserveText1] nvarchar(max)  NOT NULL,
    [ReserveText2] nvarchar(max)  NOT NULL,
    [ReserveInteger1] int  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [Operator_Id] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [BypassPositionAccess] bit  NULL,
    [Code] int  NOT NULL,
    [Title] nvarchar(max)  NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [CreatedBy] int  NOT NULL,
    [ModifiedBy] int  NOT NULL
);
GO

-- Creating table 'AccessRules'
CREATE TABLE [dbo].[AccessRules] (
    [Id] int  NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Parent_Id] int  NULL
);
GO

-- Creating table 'User_AccessRules'
CREATE TABLE [dbo].[User_AccessRules] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] tinyint  NULL,
    [ModifiedBy] int  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [User_Id] int  NOT NULL,
    [AccessRule_Id] int  NOT NULL
);
GO

-- Creating table 'Positions'
CREATE TABLE [dbo].[Positions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ModifiedDate] datetime  NOT NULL
);
GO

-- Creating table 'User_Positions'
CREATE TABLE [dbo].[User_Positions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [User_Id] int  NOT NULL,
    [Position_Id] int  NOT NULL
);
GO

-- Creating table 'Position_AccessRules'
CREATE TABLE [dbo].[Position_AccessRules] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] tinyint  NULL,
    [ModifiedBy] int  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [Position_Id] int  NOT NULL,
    [AccessRule_Id] int  NOT NULL
);
GO

-- Creating table 'ProductGroups'
CREATE TABLE [dbo].[ProductGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL
);
GO

-- Creating table 'StateStationActivities'
CREATE TABLE [dbo].[StateStationActivities] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CycleTime] real  NOT NULL,
    [ManHour] real  NOT NULL,
    [IsMany] bit  NOT NULL,
    [IsInJob] bit  NOT NULL,
    [StateStation_Id] int  NOT NULL,
    [Activity_Id] int  NOT NULL
);
GO

-- Creating table 'StateStationActivityMachines'
CREATE TABLE [dbo].[StateStationActivityMachines] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IsFixed] bit  NOT NULL,
    [StateStationActivity_Id] int  NOT NULL,
    [Machine_Id] int  NOT NULL
);
GO

-- Creating table 'OrganizationCharts'
CREATE TABLE [dbo].[OrganizationCharts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [ModifiedBy] int  NOT NULL
);
GO

-- Creating table 'OrganizationChart_Position'
CREATE TABLE [dbo].[OrganizationChart_Position] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Position_Id] int  NOT NULL,
    [OrganizationChart_Id] int  NOT NULL,
    [Parent_Id] int  NULL
);
GO

-- Creating table 'FishboneNodes'
CREATE TABLE [dbo].[FishboneNodes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Type] tinyint  NOT NULL,
    [RootType] tinyint  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [ModifiedDate] datetime  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [Root_Id] int  NOT NULL,
    [DefectionReport_Id] int  NULL,
    [Parent_Id] int  NULL
);
GO

-- Creating table 'ActionPlans'
CREATE TABLE [dbo].[ActionPlans] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL
);
GO

-- Creating table 'FishboneNode_ActionPlan'
CREATE TABLE [dbo].[FishboneNode_ActionPlan] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ActionPlan_Id] int  NOT NULL,
    [FishboneNode_Id] int  NOT NULL
);
GO

-- Creating table 'StationMachines'
CREATE TABLE [dbo].[StationMachines] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Status] tinyint  NOT NULL,
    [Machine_Id] int  NOT NULL,
    [Station_Id] int  NOT NULL
);
GO

-- Creating table 'SelectedMachines'
CREATE TABLE [dbo].[SelectedMachines] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StateStationActivityMachine_Id] int  NOT NULL,
    [Process_Id] int  NOT NULL
);
GO

-- Creating table 'Costs'
CREATE TABLE [dbo].[Costs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CostValue] float  NULL,
    [Date] datetime  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Quantity] int  NULL,
    [Status] tinyint  NOT NULL,
    [CostType] tinyint  NOT NULL,
    [Machine_Id] int  NULL,
    [CostCenter_Id] int  NOT NULL,
    [PartWarehouse_Id] int  NULL,
    [Activity_Id] int  NULL,
    [Operator_Id] int  NULL,
    [Station_Id] int  NULL
);
GO

-- Creating table 'CostCenters'
CREATE TABLE [dbo].[CostCenters] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Status] tinyint  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [SourceType] tinyint  NOT NULL
);
GO

-- Creating table 'PartWarehouses'
CREATE TABLE [dbo].[PartWarehouses] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [Status] tinyint  NOT NULL,
    [Quantity] int  NULL,
    [OriginalQuantity] int  NULL,
    [TotalCost] float  NULL,
    [PartWarehouseGroup_Id] int  NOT NULL
);
GO

-- Creating table 'PartWarehouseGroups'
CREATE TABLE [dbo].[PartWarehouseGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [Status] tinyint  NOT NULL
);
GO

-- Creating table 'Warmups'
CREATE TABLE [dbo].[Warmups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Seconds] int  NOT NULL,
    [Station_Id] int  NOT NULL,
    [ProductRework_Id] int  NOT NULL
);
GO

-- Creating table 'Changeovers'
CREATE TABLE [dbo].[Changeovers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Seconds] int  NOT NULL,
    [Station_Id] int  NOT NULL,
    [FromProductRework_Id] int  NOT NULL,
    [ToProductRework_Id] int  NOT NULL
);
GO

-- Creating table 'NonProductiveTasks'
CREATE TABLE [dbo].[NonProductiveTasks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DurationSeconds] int  NOT NULL,
    [StartDateTime] datetime  NOT NULL,
    [EndDateTime] datetime  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'EducatingOperators'
CREATE TABLE [dbo].[EducatingOperators] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Operator_Id] int  NOT NULL,
    [Education_Id] int  NOT NULL
);
GO

-- Creating table 'NonProductiveTaskReports'
CREATE TABLE [dbo].[NonProductiveTaskReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ReportDurationSeconds] int  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [NonProductiveTasks_Id] int  NOT NULL
);
GO

-- Creating table 'Jobs'
CREATE TABLE [dbo].[Jobs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Deadline] datetime  NOT NULL,
    [Weight] real  NOT NULL,
    [Quantity] int  NOT NULL,
    [Description] nvarchar(max)  NULL,
    [Code] nvarchar(max)  NULL,
    [ReleaseTime] datetime  NOT NULL,
    [ProductRework_Id] int  NOT NULL,
    [FPC_Id] int  NOT NULL
);
GO

-- Creating table 'ObjectiveFunctions'
CREATE TABLE [dbo].[ObjectiveFunctions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Weight] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'WorkProfiles'
CREATE TABLE [dbo].[WorkProfiles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [WeekStartNr] tinyint  NOT NULL,
    [SaturdayBusinessStateNr] tinyint  NOT NULL,
    [SundayBusinessStateNr] tinyint  NOT NULL,
    [MondayBusinessStateNr] tinyint  NOT NULL,
    [TuesdayBusinessStateNr] tinyint  NOT NULL,
    [WednesdayBusinessStateNr] tinyint  NOT NULL,
    [ThursdayBusinessStateNr] tinyint  NOT NULL,
    [FridayBusinessStateNr] tinyint  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NOT NULL
);
GO

-- Creating table 'WorkDays'
CREATE TABLE [dbo].[WorkDays] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ColorNr] int  NOT NULL,
    [BusinessStateNr] tinyint  NOT NULL,
    [WorkProfile_Id] int  NOT NULL
);
GO

-- Creating table 'WorkShifts'
CREATE TABLE [dbo].[WorkShifts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StartSeconds] int  NOT NULL,
    [EndSeconds] int  NOT NULL,
    [IsOpen] bit  NOT NULL,
    [WorkDay_Id] int  NOT NULL,
    [WorkShiftPrototype_Id] int  NOT NULL
);
GO

-- Creating table 'WorkBreaks'
CREATE TABLE [dbo].[WorkBreaks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StartSeconds] int  NOT NULL,
    [EndSeconds] int  NOT NULL,
    [WorkShift_Id] int  NOT NULL
);
GO

-- Creating table 'WorkProfilePlans'
CREATE TABLE [dbo].[WorkProfilePlans] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [StartDate] datetime  NOT NULL,
    [EndDate] datetime  NOT NULL,
    [MergingStrategyNr] tinyint  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [WorkProfile_Id] int  NOT NULL
);
GO

-- Creating table 'Holidays'
CREATE TABLE [dbo].[Holidays] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Date] datetime  NOT NULL,
    [BusinessStateNr] tinyint  NOT NULL,
    [IsRecurrent] bit  NOT NULL
);
GO

-- Creating table 'WorkShiftPrototypes'
CREATE TABLE [dbo].[WorkShiftPrototypes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [ColorNr] int  NOT NULL,
    [Index] tinyint  NOT NULL,
    [WorkProfile_Id] int  NOT NULL
);
GO

-- Creating table 'OperatorProcessReports'
CREATE TABLE [dbo].[OperatorProcessReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [OperatorProducedG1] int  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ProcessReport_Id] int  NOT NULL,
    [ProcessOperator_Id] int  NOT NULL
);
GO

-- Creating table 'Blocks'
CREATE TABLE [dbo].[Blocks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DurationSeconds] int  NOT NULL,
    [StartDateTime] datetime  NOT NULL,
    [Code] nvarchar(max)  NULL,
    [BlockTargetPoint] int  NOT NULL,
    [EndDateTime] datetime  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [PPFlagsNr] tinyint  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [Job_Id] int  NULL,
    [Education_Id] int  NULL,
    [StateStation_Id] int  NOT NULL
);
GO

-- Creating table 'ActivitySkills'
CREATE TABLE [dbo].[ActivitySkills] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IluoNr] tinyint  NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [Operator_Id] int  NOT NULL,
    [Activity_Id] int  NOT NULL
);
GO

-- Creating table 'ExternalConnectors'
CREATE TABLE [dbo].[ExternalConnectors] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [StartSSA_Id] int  NOT NULL,
    [EndSSA_Id] int  NOT NULL
);
GO

-- Creating table 'Parts'
CREATE TABLE [dbo].[Parts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ModifiedDate] datetime  NOT NULL
);
GO

-- Creating table 'MachineParts'
CREATE TABLE [dbo].[MachineParts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IsMachine] bit  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [Part_Id] int  NULL,
    [Machine_Id] int  NOT NULL
);
GO

-- Creating table 'Maintenances'
CREATE TABLE [dbo].[Maintenances] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL
);
GO

-- Creating table 'MachinePartMaintenances'
CREATE TABLE [dbo].[MachinePartMaintenances] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IsOnDemand] bit  NOT NULL,
    [PeriodDays] int  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [Status] tinyint  NOT NULL,
    [LastMaintenanceDate] datetime  NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Maintenance_Id] int  NOT NULL,
    [MachinePart_Id] int  NOT NULL
);
GO

-- Creating table 'MaintenanceReports'
CREATE TABLE [dbo].[MaintenanceReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MaintenanceDate] datetime  NOT NULL,
    [PerformedDate] datetime  NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Status] tinyint  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [ModifiedDate] datetime  NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [MachinePartMaintenance_Id] int  NOT NULL
);
GO

-- Creating table 'Repairs'
CREATE TABLE [dbo].[Repairs] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [CreatedDate] datetime  NOT NULL,
    [ModifiedBy] int  NOT NULL,
    [AcquiredDate] datetime  NOT NULL,
    [DeliveredDate] datetime  NOT NULL,
    [RepairStatus] tinyint  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [StoppageReport_Id] int  NULL,
    [MachinePart_Id] int  NOT NULL
);
GO

-- Creating table 'NonProductiveTasks_PM'
CREATE TABLE [dbo].[NonProductiveTasks_PM] (
    [Id] int  NOT NULL,
    [StationMachine_Id] int  NOT NULL
);
GO

-- Creating table 'NonProductiveTasks_Education'
CREATE TABLE [dbo].[NonProductiveTasks_Education] (
    [StartOffset] datetimeoffset  NOT NULL,
    [Id] int  NOT NULL
);
GO

-- Creating table 'NonProductiveTasks_Setup'
CREATE TABLE [dbo].[NonProductiveTasks_Setup] (
    [Id] int  NOT NULL,
    [Changeover_Id] int  NOT NULL,
    [Warmup_Id] int  NOT NULL
);
GO

-- Creating table 'NonProductiveTasks_Test'
CREATE TABLE [dbo].[NonProductiveTasks_Test] (
    [Id] int  NOT NULL,
    [Station_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [PK_Products]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FPCs'
ALTER TABLE [dbo].[FPCs]
ADD CONSTRAINT [PK_FPCs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProductDefections'
ALTER TABLE [dbo].[ProductDefections]
ADD CONSTRAINT [PK_ProductDefections]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Defections'
ALTER TABLE [dbo].[Defections]
ADD CONSTRAINT [PK_Defections]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Connectors'
ALTER TABLE [dbo].[Connectors]
ADD CONSTRAINT [PK_Connectors]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Causes'
ALTER TABLE [dbo].[Causes]
ADD CONSTRAINT [PK_Causes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DefectionReports'
ALTER TABLE [dbo].[DefectionReports]
ADD CONSTRAINT [PK_DefectionReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Stations'
ALTER TABLE [dbo].[Stations]
ADD CONSTRAINT [PK_Stations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [PK_Tasks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [PK_Activities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ActivityGroups'
ALTER TABLE [dbo].[ActivityGroups]
ADD CONSTRAINT [PK_ActivityGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProductActivitySkills'
ALTER TABLE [dbo].[ProductActivitySkills]
ADD CONSTRAINT [PK_ProductActivitySkills]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Operators'
ALTER TABLE [dbo].[Operators]
ADD CONSTRAINT [PK_Operators]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Machines'
ALTER TABLE [dbo].[Machines]
ADD CONSTRAINT [PK_Machines]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MachineFamilies'
ALTER TABLE [dbo].[MachineFamilies]
ADD CONSTRAINT [PK_MachineFamilies]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'States'
ALTER TABLE [dbo].[States]
ADD CONSTRAINT [PK_States]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'StateStations'
ALTER TABLE [dbo].[StateStations]
ADD CONSTRAINT [PK_StateStations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Process'
ALTER TABLE [dbo].[Process]
ADD CONSTRAINT [PK_Process]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProcessOperators'
ALTER TABLE [dbo].[ProcessOperators]
ADD CONSTRAINT [PK_ProcessOperators]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProcessReports'
ALTER TABLE [dbo].[ProcessReports]
ADD CONSTRAINT [PK_ProcessReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TaskReports'
ALTER TABLE [dbo].[TaskReports]
ADD CONSTRAINT [PK_TaskReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'StoppageReports'
ALTER TABLE [dbo].[StoppageReports]
ADD CONSTRAINT [PK_StoppageReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Operator_StoppageReports'
ALTER TABLE [dbo].[Operator_StoppageReports]
ADD CONSTRAINT [PK_Operator_StoppageReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OperatorDefectionReports'
ALTER TABLE [dbo].[OperatorDefectionReports]
ADD CONSTRAINT [PK_OperatorDefectionReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Roots'
ALTER TABLE [dbo].[Roots]
ADD CONSTRAINT [PK_Roots]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Reworks'
ALTER TABLE [dbo].[Reworks]
ADD CONSTRAINT [PK_Reworks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProductReworks'
ALTER TABLE [dbo].[ProductReworks]
ADD CONSTRAINT [PK_ProductReworks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PersonalSkills'
ALTER TABLE [dbo].[PersonalSkills]
ADD CONSTRAINT [PK_PersonalSkills]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AccessRules'
ALTER TABLE [dbo].[AccessRules]
ADD CONSTRAINT [PK_AccessRules]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'User_AccessRules'
ALTER TABLE [dbo].[User_AccessRules]
ADD CONSTRAINT [PK_User_AccessRules]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Positions'
ALTER TABLE [dbo].[Positions]
ADD CONSTRAINT [PK_Positions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'User_Positions'
ALTER TABLE [dbo].[User_Positions]
ADD CONSTRAINT [PK_User_Positions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Position_AccessRules'
ALTER TABLE [dbo].[Position_AccessRules]
ADD CONSTRAINT [PK_Position_AccessRules]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ProductGroups'
ALTER TABLE [dbo].[ProductGroups]
ADD CONSTRAINT [PK_ProductGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'StateStationActivities'
ALTER TABLE [dbo].[StateStationActivities]
ADD CONSTRAINT [PK_StateStationActivities]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'StateStationActivityMachines'
ALTER TABLE [dbo].[StateStationActivityMachines]
ADD CONSTRAINT [PK_StateStationActivityMachines]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrganizationCharts'
ALTER TABLE [dbo].[OrganizationCharts]
ADD CONSTRAINT [PK_OrganizationCharts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OrganizationChart_Position'
ALTER TABLE [dbo].[OrganizationChart_Position]
ADD CONSTRAINT [PK_OrganizationChart_Position]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FishboneNodes'
ALTER TABLE [dbo].[FishboneNodes]
ADD CONSTRAINT [PK_FishboneNodes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ActionPlans'
ALTER TABLE [dbo].[ActionPlans]
ADD CONSTRAINT [PK_ActionPlans]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FishboneNode_ActionPlan'
ALTER TABLE [dbo].[FishboneNode_ActionPlan]
ADD CONSTRAINT [PK_FishboneNode_ActionPlan]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'StationMachines'
ALTER TABLE [dbo].[StationMachines]
ADD CONSTRAINT [PK_StationMachines]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SelectedMachines'
ALTER TABLE [dbo].[SelectedMachines]
ADD CONSTRAINT [PK_SelectedMachines]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Costs'
ALTER TABLE [dbo].[Costs]
ADD CONSTRAINT [PK_Costs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CostCenters'
ALTER TABLE [dbo].[CostCenters]
ADD CONSTRAINT [PK_CostCenters]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PartWarehouses'
ALTER TABLE [dbo].[PartWarehouses]
ADD CONSTRAINT [PK_PartWarehouses]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PartWarehouseGroups'
ALTER TABLE [dbo].[PartWarehouseGroups]
ADD CONSTRAINT [PK_PartWarehouseGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Warmups'
ALTER TABLE [dbo].[Warmups]
ADD CONSTRAINT [PK_Warmups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Changeovers'
ALTER TABLE [dbo].[Changeovers]
ADD CONSTRAINT [PK_Changeovers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NonProductiveTasks'
ALTER TABLE [dbo].[NonProductiveTasks]
ADD CONSTRAINT [PK_NonProductiveTasks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EducatingOperators'
ALTER TABLE [dbo].[EducatingOperators]
ADD CONSTRAINT [PK_EducatingOperators]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NonProductiveTaskReports'
ALTER TABLE [dbo].[NonProductiveTaskReports]
ADD CONSTRAINT [PK_NonProductiveTaskReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Jobs'
ALTER TABLE [dbo].[Jobs]
ADD CONSTRAINT [PK_Jobs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ObjectiveFunctions'
ALTER TABLE [dbo].[ObjectiveFunctions]
ADD CONSTRAINT [PK_ObjectiveFunctions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WorkProfiles'
ALTER TABLE [dbo].[WorkProfiles]
ADD CONSTRAINT [PK_WorkProfiles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WorkDays'
ALTER TABLE [dbo].[WorkDays]
ADD CONSTRAINT [PK_WorkDays]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WorkShifts'
ALTER TABLE [dbo].[WorkShifts]
ADD CONSTRAINT [PK_WorkShifts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WorkBreaks'
ALTER TABLE [dbo].[WorkBreaks]
ADD CONSTRAINT [PK_WorkBreaks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WorkProfilePlans'
ALTER TABLE [dbo].[WorkProfilePlans]
ADD CONSTRAINT [PK_WorkProfilePlans]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Holidays'
ALTER TABLE [dbo].[Holidays]
ADD CONSTRAINT [PK_Holidays]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'WorkShiftPrototypes'
ALTER TABLE [dbo].[WorkShiftPrototypes]
ADD CONSTRAINT [PK_WorkShiftPrototypes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'OperatorProcessReports'
ALTER TABLE [dbo].[OperatorProcessReports]
ADD CONSTRAINT [PK_OperatorProcessReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Blocks'
ALTER TABLE [dbo].[Blocks]
ADD CONSTRAINT [PK_Blocks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ActivitySkills'
ALTER TABLE [dbo].[ActivitySkills]
ADD CONSTRAINT [PK_ActivitySkills]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExternalConnectors'
ALTER TABLE [dbo].[ExternalConnectors]
ADD CONSTRAINT [PK_ExternalConnectors]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Parts'
ALTER TABLE [dbo].[Parts]
ADD CONSTRAINT [PK_Parts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MachineParts'
ALTER TABLE [dbo].[MachineParts]
ADD CONSTRAINT [PK_MachineParts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Maintenances'
ALTER TABLE [dbo].[Maintenances]
ADD CONSTRAINT [PK_Maintenances]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MachinePartMaintenances'
ALTER TABLE [dbo].[MachinePartMaintenances]
ADD CONSTRAINT [PK_MachinePartMaintenances]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'MaintenanceReports'
ALTER TABLE [dbo].[MaintenanceReports]
ADD CONSTRAINT [PK_MaintenanceReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Repairs'
ALTER TABLE [dbo].[Repairs]
ADD CONSTRAINT [PK_Repairs]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NonProductiveTasks_PM'
ALTER TABLE [dbo].[NonProductiveTasks_PM]
ADD CONSTRAINT [PK_NonProductiveTasks_PM]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NonProductiveTasks_Education'
ALTER TABLE [dbo].[NonProductiveTasks_Education]
ADD CONSTRAINT [PK_NonProductiveTasks_Education]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NonProductiveTasks_Setup'
ALTER TABLE [dbo].[NonProductiveTasks_Setup]
ADD CONSTRAINT [PK_NonProductiveTasks_Setup]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'NonProductiveTasks_Test'
ALTER TABLE [dbo].[NonProductiveTasks_Test]
ADD CONSTRAINT [PK_NonProductiveTasks_Test]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ActivityGroup_Id] in table 'Activities'
ALTER TABLE [dbo].[Activities]
ADD CONSTRAINT [FK_ActivityGroupActivity]
    FOREIGN KEY ([ActivityGroup_Id])
    REFERENCES [dbo].[ActivityGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityGroupActivity'
CREATE INDEX [IX_FK_ActivityGroupActivity]
ON [dbo].[Activities]
    ([ActivityGroup_Id]);
GO

-- Creating foreign key on [MachineFamily_Id] in table 'Machines'
ALTER TABLE [dbo].[Machines]
ADD CONSTRAINT [FK_MachineFamilyMachine]
    FOREIGN KEY ([MachineFamily_Id])
    REFERENCES [dbo].[MachineFamilies]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MachineFamilyMachine'
CREATE INDEX [IX_FK_MachineFamilyMachine]
ON [dbo].[Machines]
    ([MachineFamily_Id]);
GO

-- Creating foreign key on [State_Id] in table 'StateStations'
ALTER TABLE [dbo].[StateStations]
ADD CONSTRAINT [FK_StateStateStation]
    FOREIGN KEY ([State_Id])
    REFERENCES [dbo].[States]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StateStateStation'
CREATE INDEX [IX_FK_StateStateStation]
ON [dbo].[StateStations]
    ([State_Id]);
GO

-- Creating foreign key on [StartState_Id] in table 'Connectors'
ALTER TABLE [dbo].[Connectors]
ADD CONSTRAINT [FK_StateConnector]
    FOREIGN KEY ([StartState_Id])
    REFERENCES [dbo].[States]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StateConnector'
CREATE INDEX [IX_FK_StateConnector]
ON [dbo].[Connectors]
    ([StartState_Id]);
GO

-- Creating foreign key on [EndState_Id] in table 'Connectors'
ALTER TABLE [dbo].[Connectors]
ADD CONSTRAINT [FK_StateConnector1]
    FOREIGN KEY ([EndState_Id])
    REFERENCES [dbo].[States]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StateConnector1'
CREATE INDEX [IX_FK_StateConnector1]
ON [dbo].[Connectors]
    ([EndState_Id]);
GO

-- Creating foreign key on [Product_Id] in table 'ProductDefections'
ALTER TABLE [dbo].[ProductDefections]
ADD CONSTRAINT [FK_ProductProductDefection]
    FOREIGN KEY ([Product_Id])
    REFERENCES [dbo].[Products]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductProductDefection'
CREATE INDEX [IX_FK_ProductProductDefection]
ON [dbo].[ProductDefections]
    ([Product_Id]);
GO

-- Creating foreign key on [Defection_Id] in table 'ProductDefections'
ALTER TABLE [dbo].[ProductDefections]
ADD CONSTRAINT [FK_DefectionProductDefection]
    FOREIGN KEY ([Defection_Id])
    REFERENCES [dbo].[Defections]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DefectionProductDefection'
CREATE INDEX [IX_FK_DefectionProductDefection]
ON [dbo].[ProductDefections]
    ([Defection_Id]);
GO

-- Creating foreign key on [User_Id] in table 'User_Positions'
ALTER TABLE [dbo].[User_Positions]
ADD CONSTRAINT [FK_UserUser_UserGroup]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUser_UserGroup'
CREATE INDEX [IX_FK_UserUser_UserGroup]
ON [dbo].[User_Positions]
    ([User_Id]);
GO

-- Creating foreign key on [Position_Id] in table 'User_Positions'
ALTER TABLE [dbo].[User_Positions]
ADD CONSTRAINT [FK_UserGroupUser_UserGroup]
    FOREIGN KEY ([Position_Id])
    REFERENCES [dbo].[Positions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserGroupUser_UserGroup'
CREATE INDEX [IX_FK_UserGroupUser_UserGroup]
ON [dbo].[User_Positions]
    ([Position_Id]);
GO

-- Creating foreign key on [Position_Id] in table 'Position_AccessRules'
ALTER TABLE [dbo].[Position_AccessRules]
ADD CONSTRAINT [FK_UserGroupUserGroup_AccessRule]
    FOREIGN KEY ([Position_Id])
    REFERENCES [dbo].[Positions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserGroupUserGroup_AccessRule'
CREATE INDEX [IX_FK_UserGroupUserGroup_AccessRule]
ON [dbo].[Position_AccessRules]
    ([Position_Id]);
GO

-- Creating foreign key on [User_Id] in table 'User_AccessRules'
ALTER TABLE [dbo].[User_AccessRules]
ADD CONSTRAINT [FK_UserUser_AccessRule]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserUser_AccessRule'
CREATE INDEX [IX_FK_UserUser_AccessRule]
ON [dbo].[User_AccessRules]
    ([User_Id]);
GO

-- Creating foreign key on [ProductGroup_Id] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [FK_ProductGroupProduct]
    FOREIGN KEY ([ProductGroup_Id])
    REFERENCES [dbo].[ProductGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductGroupProduct'
CREATE INDEX [IX_FK_ProductGroupProduct]
ON [dbo].[Products]
    ([ProductGroup_Id]);
GO

-- Creating foreign key on [Position_Id] in table 'OrganizationChart_Position'
ALTER TABLE [dbo].[OrganizationChart_Position]
ADD CONSTRAINT [FK_PositionOrganizationChart_Position]
    FOREIGN KEY ([Position_Id])
    REFERENCES [dbo].[Positions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PositionOrganizationChart_Position'
CREATE INDEX [IX_FK_PositionOrganizationChart_Position]
ON [dbo].[OrganizationChart_Position]
    ([Position_Id]);
GO

-- Creating foreign key on [OrganizationChart_Id] in table 'OrganizationChart_Position'
ALTER TABLE [dbo].[OrganizationChart_Position]
ADD CONSTRAINT [FK_OrganizationChartOrganizationChart_Position]
    FOREIGN KEY ([OrganizationChart_Id])
    REFERENCES [dbo].[OrganizationCharts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrganizationChartOrganizationChart_Position'
CREATE INDEX [IX_FK_OrganizationChartOrganizationChart_Position]
ON [dbo].[OrganizationChart_Position]
    ([OrganizationChart_Id]);
GO

-- Creating foreign key on [AccessRule_Id] in table 'User_AccessRules'
ALTER TABLE [dbo].[User_AccessRules]
ADD CONSTRAINT [FK_AccessRuleUser_AccessRule]
    FOREIGN KEY ([AccessRule_Id])
    REFERENCES [dbo].[AccessRules]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AccessRuleUser_AccessRule'
CREATE INDEX [IX_FK_AccessRuleUser_AccessRule]
ON [dbo].[User_AccessRules]
    ([AccessRule_Id]);
GO

-- Creating foreign key on [AccessRule_Id] in table 'Position_AccessRules'
ALTER TABLE [dbo].[Position_AccessRules]
ADD CONSTRAINT [FK_AccessRulePosition_AccessRule]
    FOREIGN KEY ([AccessRule_Id])
    REFERENCES [dbo].[AccessRules]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AccessRulePosition_AccessRule'
CREATE INDEX [IX_FK_AccessRulePosition_AccessRule]
ON [dbo].[Position_AccessRules]
    ([AccessRule_Id]);
GO

-- Creating foreign key on [Parent_Id] in table 'OrganizationChart_Position'
ALTER TABLE [dbo].[OrganizationChart_Position]
ADD CONSTRAINT [FK_OrganizationChart_PositionOrganizationChart_Position]
    FOREIGN KEY ([Parent_Id])
    REFERENCES [dbo].[OrganizationChart_Position]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OrganizationChart_PositionOrganizationChart_Position'
CREATE INDEX [IX_FK_OrganizationChart_PositionOrganizationChart_Position]
ON [dbo].[OrganizationChart_Position]
    ([Parent_Id]);
GO

-- Creating foreign key on [Parent_Id] in table 'AccessRules'
ALTER TABLE [dbo].[AccessRules]
ADD CONSTRAINT [FK_AccessRuleAccessRule]
    FOREIGN KEY ([Parent_Id])
    REFERENCES [dbo].[AccessRules]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AccessRuleAccessRule'
CREATE INDEX [IX_FK_AccessRuleAccessRule]
ON [dbo].[AccessRules]
    ([Parent_Id]);
GO

-- Creating foreign key on [Parent_Id] in table 'Products'
ALTER TABLE [dbo].[Products]
ADD CONSTRAINT [FK_ProductProduct]
    FOREIGN KEY ([Parent_Id])
    REFERENCES [dbo].[Products]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductProduct'
CREATE INDEX [IX_FK_ProductProduct]
ON [dbo].[Products]
    ([Parent_Id]);
GO

-- Creating foreign key on [Root_Id] in table 'FishboneNodes'
ALTER TABLE [dbo].[FishboneNodes]
ADD CONSTRAINT [FK_RootProductDefection_Root]
    FOREIGN KEY ([Root_Id])
    REFERENCES [dbo].[Roots]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_RootProductDefection_Root'
CREATE INDEX [IX_FK_RootProductDefection_Root]
ON [dbo].[FishboneNodes]
    ([Root_Id]);
GO

-- Creating foreign key on [DefectionReport_Id] in table 'FishboneNodes'
ALTER TABLE [dbo].[FishboneNodes]
ADD CONSTRAINT [FK_DefectionReportProductDefection_Root]
    FOREIGN KEY ([DefectionReport_Id])
    REFERENCES [dbo].[DefectionReports]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DefectionReportProductDefection_Root'
CREATE INDEX [IX_FK_DefectionReportProductDefection_Root]
ON [dbo].[FishboneNodes]
    ([DefectionReport_Id]);
GO

-- Creating foreign key on [Parent_Id] in table 'FishboneNodes'
ALTER TABLE [dbo].[FishboneNodes]
ADD CONSTRAINT [FK_ProductDefection_RootProductDefection_Root]
    FOREIGN KEY ([Parent_Id])
    REFERENCES [dbo].[FishboneNodes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductDefection_RootProductDefection_Root'
CREATE INDEX [IX_FK_ProductDefection_RootProductDefection_Root]
ON [dbo].[FishboneNodes]
    ([Parent_Id]);
GO

-- Creating foreign key on [ActionPlan_Id] in table 'FishboneNode_ActionPlan'
ALTER TABLE [dbo].[FishboneNode_ActionPlan]
ADD CONSTRAINT [FK_ActionPlanProductDefectionRoot_ActionPlan]
    FOREIGN KEY ([ActionPlan_Id])
    REFERENCES [dbo].[ActionPlans]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActionPlanProductDefectionRoot_ActionPlan'
CREATE INDEX [IX_FK_ActionPlanProductDefectionRoot_ActionPlan]
ON [dbo].[FishboneNode_ActionPlan]
    ([ActionPlan_Id]);
GO

-- Creating foreign key on [FishboneNode_Id] in table 'FishboneNode_ActionPlan'
ALTER TABLE [dbo].[FishboneNode_ActionPlan]
ADD CONSTRAINT [FK_ProductDefection_RootProductDefectionRoot_ActionPlan]
    FOREIGN KEY ([FishboneNode_Id])
    REFERENCES [dbo].[FishboneNodes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductDefection_RootProductDefectionRoot_ActionPlan'
CREATE INDEX [IX_FK_ProductDefection_RootProductDefectionRoot_ActionPlan]
ON [dbo].[FishboneNode_ActionPlan]
    ([FishboneNode_Id]);
GO

-- Creating foreign key on [Machine_Id] in table 'StationMachines'
ALTER TABLE [dbo].[StationMachines]
ADD CONSTRAINT [FK_MachineStationMachine]
    FOREIGN KEY ([Machine_Id])
    REFERENCES [dbo].[Machines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MachineStationMachine'
CREATE INDEX [IX_FK_MachineStationMachine]
ON [dbo].[StationMachines]
    ([Machine_Id]);
GO

-- Creating foreign key on [FPC_Id] in table 'States'
ALTER TABLE [dbo].[States]
ADD CONSTRAINT [FK_FPCState]
    FOREIGN KEY ([FPC_Id])
    REFERENCES [dbo].[FPCs]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FPCState'
CREATE INDEX [IX_FK_FPCState]
ON [dbo].[States]
    ([FPC_Id]);
GO

-- Creating foreign key on [Product_Id] in table 'ProductReworks'
ALTER TABLE [dbo].[ProductReworks]
ADD CONSTRAINT [FK_ProductProductRework]
    FOREIGN KEY ([Product_Id])
    REFERENCES [dbo].[Products]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductProductRework'
CREATE INDEX [IX_FK_ProductProductRework]
ON [dbo].[ProductReworks]
    ([Product_Id]);
GO

-- Creating foreign key on [Rework_Id] in table 'ProductReworks'
ALTER TABLE [dbo].[ProductReworks]
ADD CONSTRAINT [FK_ReworkProductRework]
    FOREIGN KEY ([Rework_Id])
    REFERENCES [dbo].[Reworks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ReworkProductRework'
CREATE INDEX [IX_FK_ReworkProductRework]
ON [dbo].[ProductReworks]
    ([Rework_Id]);
GO

-- Creating foreign key on [ActionPlan_Id] in table 'DefectionReports'
ALTER TABLE [dbo].[DefectionReports]
ADD CONSTRAINT [FK_ActionPlanDefectionReport]
    FOREIGN KEY ([ActionPlan_Id])
    REFERENCES [dbo].[ActionPlans]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActionPlanDefectionReport'
CREATE INDEX [IX_FK_ActionPlanDefectionReport]
ON [dbo].[DefectionReports]
    ([ActionPlan_Id]);
GO

-- Creating foreign key on [StateStation_Id] in table 'StateStationActivities'
ALTER TABLE [dbo].[StateStationActivities]
ADD CONSTRAINT [FK_StateStationStateStationActivity]
    FOREIGN KEY ([StateStation_Id])
    REFERENCES [dbo].[StateStations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StateStationStateStationActivity'
CREATE INDEX [IX_FK_StateStationStateStationActivity]
ON [dbo].[StateStationActivities]
    ([StateStation_Id]);
GO

-- Creating foreign key on [StateStationActivity_Id] in table 'StateStationActivityMachines'
ALTER TABLE [dbo].[StateStationActivityMachines]
ADD CONSTRAINT [FK_StateStationActivityStateStationActivityMachine]
    FOREIGN KEY ([StateStationActivity_Id])
    REFERENCES [dbo].[StateStationActivities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StateStationActivityStateStationActivityMachine'
CREATE INDEX [IX_FK_StateStationActivityStateStationActivityMachine]
ON [dbo].[StateStationActivityMachines]
    ([StateStationActivity_Id]);
GO

-- Creating foreign key on [Machine_Id] in table 'StateStationActivityMachines'
ALTER TABLE [dbo].[StateStationActivityMachines]
ADD CONSTRAINT [FK_MachineStateStationActivityMachine]
    FOREIGN KEY ([Machine_Id])
    REFERENCES [dbo].[Machines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MachineStateStationActivityMachine'
CREATE INDEX [IX_FK_MachineStateStationActivityMachine]
ON [dbo].[StateStationActivityMachines]
    ([Machine_Id]);
GO

-- Creating foreign key on [StateStationActivityMachine_Id] in table 'SelectedMachines'
ALTER TABLE [dbo].[SelectedMachines]
ADD CONSTRAINT [FK_StateStationActivityMachineSelectedMachine]
    FOREIGN KEY ([StateStationActivityMachine_Id])
    REFERENCES [dbo].[StateStationActivityMachines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StateStationActivityMachineSelectedMachine'
CREATE INDEX [IX_FK_StateStationActivityMachineSelectedMachine]
ON [dbo].[SelectedMachines]
    ([StateStationActivityMachine_Id]);
GO

-- Creating foreign key on [Process_Id] in table 'ProcessOperators'
ALTER TABLE [dbo].[ProcessOperators]
ADD CONSTRAINT [FK_ProcessProcessOperator]
    FOREIGN KEY ([Process_Id])
    REFERENCES [dbo].[Process]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProcessProcessOperator'
CREATE INDEX [IX_FK_ProcessProcessOperator]
ON [dbo].[ProcessOperators]
    ([Process_Id]);
GO

-- Creating foreign key on [Process_Id] in table 'SelectedMachines'
ALTER TABLE [dbo].[SelectedMachines]
ADD CONSTRAINT [FK_ProcessSelectedMachine]
    FOREIGN KEY ([Process_Id])
    REFERENCES [dbo].[Process]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProcessSelectedMachine'
CREATE INDEX [IX_FK_ProcessSelectedMachine]
ON [dbo].[SelectedMachines]
    ([Process_Id]);
GO

-- Creating foreign key on [Task_Id] in table 'Process'
ALTER TABLE [dbo].[Process]
ADD CONSTRAINT [FK_TaskProcess]
    FOREIGN KEY ([Task_Id])
    REFERENCES [dbo].[Tasks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskProcess'
CREATE INDEX [IX_FK_TaskProcess]
ON [dbo].[Process]
    ([Task_Id]);
GO

-- Creating foreign key on [Operator_Id] in table 'ProcessOperators'
ALTER TABLE [dbo].[ProcessOperators]
ADD CONSTRAINT [FK_OperatorProcessOperator]
    FOREIGN KEY ([Operator_Id])
    REFERENCES [dbo].[Operators]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OperatorProcessOperator'
CREATE INDEX [IX_FK_OperatorProcessOperator]
ON [dbo].[ProcessOperators]
    ([Operator_Id]);
GO

-- Creating foreign key on [Operator_Id] in table 'Operator_StoppageReports'
ALTER TABLE [dbo].[Operator_StoppageReports]
ADD CONSTRAINT [FK_OperatorOperatorStoppageReport]
    FOREIGN KEY ([Operator_Id])
    REFERENCES [dbo].[Operators]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OperatorOperatorStoppageReport'
CREATE INDEX [IX_FK_OperatorOperatorStoppageReport]
ON [dbo].[Operator_StoppageReports]
    ([Operator_Id]);
GO

-- Creating foreign key on [StoppageReport_Id] in table 'Operator_StoppageReports'
ALTER TABLE [dbo].[Operator_StoppageReports]
ADD CONSTRAINT [FK_StoppageReportOperatorStoppageReport]
    FOREIGN KEY ([StoppageReport_Id])
    REFERENCES [dbo].[StoppageReports]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StoppageReportOperatorStoppageReport'
CREATE INDEX [IX_FK_StoppageReportOperatorStoppageReport]
ON [dbo].[Operator_StoppageReports]
    ([StoppageReport_Id]);
GO

-- Creating foreign key on [ProcessReport_Id] in table 'StoppageReports'
ALTER TABLE [dbo].[StoppageReports]
ADD CONSTRAINT [FK_ProcessReportStoppageReport]
    FOREIGN KEY ([ProcessReport_Id])
    REFERENCES [dbo].[ProcessReports]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProcessReportStoppageReport'
CREATE INDEX [IX_FK_ProcessReportStoppageReport]
ON [dbo].[StoppageReports]
    ([ProcessReport_Id]);
GO

-- Creating foreign key on [Task_Id] in table 'TaskReports'
ALTER TABLE [dbo].[TaskReports]
ADD CONSTRAINT [FK_TaskTaskReport]
    FOREIGN KEY ([Task_Id])
    REFERENCES [dbo].[Tasks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TaskTaskReport'
CREATE INDEX [IX_FK_TaskTaskReport]
ON [dbo].[TaskReports]
    ([Task_Id]);
GO

-- Creating foreign key on [ProcessReport_Id] in table 'DefectionReports'
ALTER TABLE [dbo].[DefectionReports]
ADD CONSTRAINT [FK_ProcessReportDefectionReport]
    FOREIGN KEY ([ProcessReport_Id])
    REFERENCES [dbo].[ProcessReports]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProcessReportDefectionReport'
CREATE INDEX [IX_FK_ProcessReportDefectionReport]
ON [dbo].[DefectionReports]
    ([ProcessReport_Id]);
GO

-- Creating foreign key on [DefectionReport_Id] in table 'OperatorDefectionReports'
ALTER TABLE [dbo].[OperatorDefectionReports]
ADD CONSTRAINT [FK_DefectionReportOperatorDefectionReport]
    FOREIGN KEY ([DefectionReport_Id])
    REFERENCES [dbo].[DefectionReports]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_DefectionReportOperatorDefectionReport'
CREATE INDEX [IX_FK_DefectionReportOperatorDefectionReport]
ON [dbo].[OperatorDefectionReports]
    ([DefectionReport_Id]);
GO

-- Creating foreign key on [Machine_Id] in table 'Costs'
ALTER TABLE [dbo].[Costs]
ADD CONSTRAINT [FK_MachineCost]
    FOREIGN KEY ([Machine_Id])
    REFERENCES [dbo].[Machines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MachineCost'
CREATE INDEX [IX_FK_MachineCost]
ON [dbo].[Costs]
    ([Machine_Id]);
GO

-- Creating foreign key on [CostCenter_Id] in table 'Costs'
ALTER TABLE [dbo].[Costs]
ADD CONSTRAINT [FK_CostCenterCost]
    FOREIGN KEY ([CostCenter_Id])
    REFERENCES [dbo].[CostCenters]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CostCenterCost'
CREATE INDEX [IX_FK_CostCenterCost]
ON [dbo].[Costs]
    ([CostCenter_Id]);
GO

-- Creating foreign key on [PartWarehouseGroup_Id] in table 'PartWarehouses'
ALTER TABLE [dbo].[PartWarehouses]
ADD CONSTRAINT [FK_PartsWarehouseGroupPartsWarehouse]
    FOREIGN KEY ([PartWarehouseGroup_Id])
    REFERENCES [dbo].[PartWarehouseGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PartsWarehouseGroupPartsWarehouse'
CREATE INDEX [IX_FK_PartsWarehouseGroupPartsWarehouse]
ON [dbo].[PartWarehouses]
    ([PartWarehouseGroup_Id]);
GO

-- Creating foreign key on [PartWarehouse_Id] in table 'Costs'
ALTER TABLE [dbo].[Costs]
ADD CONSTRAINT [FK_PartsWarehouseCost]
    FOREIGN KEY ([PartWarehouse_Id])
    REFERENCES [dbo].[PartWarehouses]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PartsWarehouseCost'
CREATE INDEX [IX_FK_PartsWarehouseCost]
ON [dbo].[Costs]
    ([PartWarehouse_Id]);
GO

-- Creating foreign key on [ProductDefection_Id] in table 'Roots'
ALTER TABLE [dbo].[Roots]
ADD CONSTRAINT [FK_ProductDefectionRoot]
    FOREIGN KEY ([ProductDefection_Id])
    REFERENCES [dbo].[ProductDefections]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductDefectionRoot'
CREATE INDEX [IX_FK_ProductDefectionRoot]
ON [dbo].[Roots]
    ([ProductDefection_Id]);
GO

-- Creating foreign key on [Cause_Id] in table 'StoppageReports'
ALTER TABLE [dbo].[StoppageReports]
ADD CONSTRAINT [FK_CauseStoppageReport]
    FOREIGN KEY ([Cause_Id])
    REFERENCES [dbo].[Causes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CauseStoppageReport'
CREATE INDEX [IX_FK_CauseStoppageReport]
ON [dbo].[StoppageReports]
    ([Cause_Id]);
GO

-- Creating foreign key on [Parent_Id] in table 'Causes'
ALTER TABLE [dbo].[Causes]
ADD CONSTRAINT [FK_CauseCause]
    FOREIGN KEY ([Parent_Id])
    REFERENCES [dbo].[Causes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_CauseCause'
CREATE INDEX [IX_FK_CauseCause]
ON [dbo].[Causes]
    ([Parent_Id]);
GO

-- Creating foreign key on [Station_Id] in table 'Changeovers'
ALTER TABLE [dbo].[Changeovers]
ADD CONSTRAINT [FK_StationChangeover]
    FOREIGN KEY ([Station_Id])
    REFERENCES [dbo].[Stations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StationChangeover'
CREATE INDEX [IX_FK_StationChangeover]
ON [dbo].[Changeovers]
    ([Station_Id]);
GO

-- Creating foreign key on [Station_Id] in table 'Warmups'
ALTER TABLE [dbo].[Warmups]
ADD CONSTRAINT [FK_StationWarmup]
    FOREIGN KEY ([Station_Id])
    REFERENCES [dbo].[Stations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StationWarmup'
CREATE INDEX [IX_FK_StationWarmup]
ON [dbo].[Warmups]
    ([Station_Id]);
GO

-- Creating foreign key on [FromProductRework_Id] in table 'Changeovers'
ALTER TABLE [dbo].[Changeovers]
ADD CONSTRAINT [FK_ProductReworkChangeover]
    FOREIGN KEY ([FromProductRework_Id])
    REFERENCES [dbo].[ProductReworks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductReworkChangeover'
CREATE INDEX [IX_FK_ProductReworkChangeover]
ON [dbo].[Changeovers]
    ([FromProductRework_Id]);
GO

-- Creating foreign key on [ProductRework_Id] in table 'Warmups'
ALTER TABLE [dbo].[Warmups]
ADD CONSTRAINT [FK_ProductReworkWarmup]
    FOREIGN KEY ([ProductRework_Id])
    REFERENCES [dbo].[ProductReworks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductReworkWarmup'
CREATE INDEX [IX_FK_ProductReworkWarmup]
ON [dbo].[Warmups]
    ([ProductRework_Id]);
GO

-- Creating foreign key on [StationMachine_Id] in table 'NonProductiveTasks_PM'
ALTER TABLE [dbo].[NonProductiveTasks_PM]
ADD CONSTRAINT [FK_StationMachinePM]
    FOREIGN KEY ([StationMachine_Id])
    REFERENCES [dbo].[StationMachines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StationMachinePM'
CREATE INDEX [IX_FK_StationMachinePM]
ON [dbo].[NonProductiveTasks_PM]
    ([StationMachine_Id]);
GO

-- Creating foreign key on [Operator_Id] in table 'EducatingOperators'
ALTER TABLE [dbo].[EducatingOperators]
ADD CONSTRAINT [FK_OperatorEducatingOperator]
    FOREIGN KEY ([Operator_Id])
    REFERENCES [dbo].[Operators]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OperatorEducatingOperator'
CREATE INDEX [IX_FK_OperatorEducatingOperator]
ON [dbo].[EducatingOperators]
    ([Operator_Id]);
GO

-- Creating foreign key on [Education_Id] in table 'EducatingOperators'
ALTER TABLE [dbo].[EducatingOperators]
ADD CONSTRAINT [FK_EducationEducatingOperator]
    FOREIGN KEY ([Education_Id])
    REFERENCES [dbo].[NonProductiveTasks_Education]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EducationEducatingOperator'
CREATE INDEX [IX_FK_EducationEducatingOperator]
ON [dbo].[EducatingOperators]
    ([Education_Id]);
GO

-- Creating foreign key on [Changeover_Id] in table 'NonProductiveTasks_Setup'
ALTER TABLE [dbo].[NonProductiveTasks_Setup]
ADD CONSTRAINT [FK_ChangeoverSetup]
    FOREIGN KEY ([Changeover_Id])
    REFERENCES [dbo].[Changeovers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ChangeoverSetup'
CREATE INDEX [IX_FK_ChangeoverSetup]
ON [dbo].[NonProductiveTasks_Setup]
    ([Changeover_Id]);
GO

-- Creating foreign key on [Warmup_Id] in table 'NonProductiveTasks_Setup'
ALTER TABLE [dbo].[NonProductiveTasks_Setup]
ADD CONSTRAINT [FK_WarmupSetup]
    FOREIGN KEY ([Warmup_Id])
    REFERENCES [dbo].[Warmups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WarmupSetup'
CREATE INDEX [IX_FK_WarmupSetup]
ON [dbo].[NonProductiveTasks_Setup]
    ([Warmup_Id]);
GO

-- Creating foreign key on [NonProductiveTasks_Id] in table 'NonProductiveTaskReports'
ALTER TABLE [dbo].[NonProductiveTaskReports]
ADD CONSTRAINT [FK_NonProductiveTaskReportNonProductiveTask]
    FOREIGN KEY ([NonProductiveTasks_Id])
    REFERENCES [dbo].[NonProductiveTasks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_NonProductiveTaskReportNonProductiveTask'
CREATE INDEX [IX_FK_NonProductiveTaskReportNonProductiveTask]
ON [dbo].[NonProductiveTaskReports]
    ([NonProductiveTasks_Id]);
GO

-- Creating foreign key on [ProductRework_Id] in table 'Jobs'
ALTER TABLE [dbo].[Jobs]
ADD CONSTRAINT [FK_ProductReworkJob]
    FOREIGN KEY ([ProductRework_Id])
    REFERENCES [dbo].[ProductReworks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductReworkJob'
CREATE INDEX [IX_FK_ProductReworkJob]
ON [dbo].[Jobs]
    ([ProductRework_Id]);
GO

-- Creating foreign key on [Product_Id] in table 'FPCs'
ALTER TABLE [dbo].[FPCs]
ADD CONSTRAINT [FK_ProductFPC]
    FOREIGN KEY ([Product_Id])
    REFERENCES [dbo].[Products]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductFPC'
CREATE INDEX [IX_FK_ProductFPC]
ON [dbo].[FPCs]
    ([Product_Id]);
GO

-- Creating foreign key on [OnProductRework_Id] in table 'States'
ALTER TABLE [dbo].[States]
ADD CONSTRAINT [FK_ProductReworkState]
    FOREIGN KEY ([OnProductRework_Id])
    REFERENCES [dbo].[ProductReworks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductReworkState'
CREATE INDEX [IX_FK_ProductReworkState]
ON [dbo].[States]
    ([OnProductRework_Id]);
GO

-- Creating foreign key on [Station_Id] in table 'StationMachines'
ALTER TABLE [dbo].[StationMachines]
ADD CONSTRAINT [FK_StationStationMachine]
    FOREIGN KEY ([Station_Id])
    REFERENCES [dbo].[Stations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StationStationMachine'
CREATE INDEX [IX_FK_StationStationMachine]
ON [dbo].[StationMachines]
    ([Station_Id]);
GO

-- Creating foreign key on [Activity_Id] in table 'Costs'
ALTER TABLE [dbo].[Costs]
ADD CONSTRAINT [FK_ActivityCost]
    FOREIGN KEY ([Activity_Id])
    REFERENCES [dbo].[Activities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityCost'
CREATE INDEX [IX_FK_ActivityCost]
ON [dbo].[Costs]
    ([Activity_Id]);
GO

-- Creating foreign key on [ToProductRework_Id] in table 'Changeovers'
ALTER TABLE [dbo].[Changeovers]
ADD CONSTRAINT [FK_ProductReworkChangeover1]
    FOREIGN KEY ([ToProductRework_Id])
    REFERENCES [dbo].[ProductReworks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductReworkChangeover1'
CREATE INDEX [IX_FK_ProductReworkChangeover1]
ON [dbo].[Changeovers]
    ([ToProductRework_Id]);
GO

-- Creating foreign key on [WorkShift_Id] in table 'WorkBreaks'
ALTER TABLE [dbo].[WorkBreaks]
ADD CONSTRAINT [FK_WorkShiftWorkBreak]
    FOREIGN KEY ([WorkShift_Id])
    REFERENCES [dbo].[WorkShifts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkShiftWorkBreak'
CREATE INDEX [IX_FK_WorkShiftWorkBreak]
ON [dbo].[WorkBreaks]
    ([WorkShift_Id]);
GO

-- Creating foreign key on [WorkDay_Id] in table 'WorkShifts'
ALTER TABLE [dbo].[WorkShifts]
ADD CONSTRAINT [FK_WorkDayWorkShift]
    FOREIGN KEY ([WorkDay_Id])
    REFERENCES [dbo].[WorkDays]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkDayWorkShift'
CREATE INDEX [IX_FK_WorkDayWorkShift]
ON [dbo].[WorkShifts]
    ([WorkDay_Id]);
GO

-- Creating foreign key on [WorkProfile_Id] in table 'WorkDays'
ALTER TABLE [dbo].[WorkDays]
ADD CONSTRAINT [FK_WorkProfileWorkDay]
    FOREIGN KEY ([WorkProfile_Id])
    REFERENCES [dbo].[WorkProfiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkProfileWorkDay'
CREATE INDEX [IX_FK_WorkProfileWorkDay]
ON [dbo].[WorkDays]
    ([WorkProfile_Id]);
GO

-- Creating foreign key on [WorkProfile_Id] in table 'WorkProfilePlans'
ALTER TABLE [dbo].[WorkProfilePlans]
ADD CONSTRAINT [FK_WorkProfileWorkProfilePlan]
    FOREIGN KEY ([WorkProfile_Id])
    REFERENCES [dbo].[WorkProfiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkProfileWorkProfilePlan'
CREATE INDEX [IX_FK_WorkProfileWorkProfilePlan]
ON [dbo].[WorkProfilePlans]
    ([WorkProfile_Id]);
GO

-- Creating foreign key on [WorkProfile_Id] in table 'WorkShiftPrototypes'
ALTER TABLE [dbo].[WorkShiftPrototypes]
ADD CONSTRAINT [FK_WorkProfileWorkShiftPrototype]
    FOREIGN KEY ([WorkProfile_Id])
    REFERENCES [dbo].[WorkProfiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkProfileWorkShiftPrototype'
CREATE INDEX [IX_FK_WorkProfileWorkShiftPrototype]
ON [dbo].[WorkShiftPrototypes]
    ([WorkProfile_Id]);
GO

-- Creating foreign key on [WorkShiftPrototype_Id] in table 'WorkShifts'
ALTER TABLE [dbo].[WorkShifts]
ADD CONSTRAINT [FK_WorkShiftPrototypeWorkShift]
    FOREIGN KEY ([WorkShiftPrototype_Id])
    REFERENCES [dbo].[WorkShiftPrototypes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_WorkShiftPrototypeWorkShift'
CREATE INDEX [IX_FK_WorkShiftPrototypeWorkShift]
ON [dbo].[WorkShifts]
    ([WorkShiftPrototype_Id]);
GO

-- Creating foreign key on [Activity_Id] in table 'StateStationActivities'
ALTER TABLE [dbo].[StateStationActivities]
ADD CONSTRAINT [FK_ActivityStateStationActivity]
    FOREIGN KEY ([Activity_Id])
    REFERENCES [dbo].[Activities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityStateStationActivity'
CREATE INDEX [IX_FK_ActivityStateStationActivity]
ON [dbo].[StateStationActivities]
    ([Activity_Id]);
GO

-- Creating foreign key on [StateStationActivity_Id] in table 'Process'
ALTER TABLE [dbo].[Process]
ADD CONSTRAINT [FK_StateStationActivityProcess]
    FOREIGN KEY ([StateStationActivity_Id])
    REFERENCES [dbo].[StateStationActivities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StateStationActivityProcess'
CREATE INDEX [IX_FK_StateStationActivityProcess]
ON [dbo].[Process]
    ([StateStationActivity_Id]);
GO

-- Creating foreign key on [Operator_Id] in table 'Costs'
ALTER TABLE [dbo].[Costs]
ADD CONSTRAINT [FK_OperatorCost]
    FOREIGN KEY ([Operator_Id])
    REFERENCES [dbo].[Operators]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OperatorCost'
CREATE INDEX [IX_FK_OperatorCost]
ON [dbo].[Costs]
    ([Operator_Id]);
GO

-- Creating foreign key on [ProductDefection_Id] in table 'DefectionReports'
ALTER TABLE [dbo].[DefectionReports]
ADD CONSTRAINT [FK_ProductDefectionDefectionReport]
    FOREIGN KEY ([ProductDefection_Id])
    REFERENCES [dbo].[ProductDefections]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductDefectionDefectionReport'
CREATE INDEX [IX_FK_ProductDefectionDefectionReport]
ON [dbo].[DefectionReports]
    ([ProductDefection_Id]);
GO

-- Creating foreign key on [Operator_Id] in table 'PersonalSkills'
ALTER TABLE [dbo].[PersonalSkills]
ADD CONSTRAINT [FK_OperatorPersonalSkill]
    FOREIGN KEY ([Operator_Id])
    REFERENCES [dbo].[Operators]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OperatorPersonalSkill'
CREATE INDEX [IX_FK_OperatorPersonalSkill]
ON [dbo].[PersonalSkills]
    ([Operator_Id]);
GO

-- Creating foreign key on [Operator_Id] in table 'OperatorDefectionReports'
ALTER TABLE [dbo].[OperatorDefectionReports]
ADD CONSTRAINT [FK_OperatorOperatorDefectionReport]
    FOREIGN KEY ([Operator_Id])
    REFERENCES [dbo].[Operators]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OperatorOperatorDefectionReport'
CREATE INDEX [IX_FK_OperatorOperatorDefectionReport]
ON [dbo].[OperatorDefectionReports]
    ([Operator_Id]);
GO

-- Creating foreign key on [FPC_Id] in table 'Jobs'
ALTER TABLE [dbo].[Jobs]
ADD CONSTRAINT [FK_FPCJob]
    FOREIGN KEY ([FPC_Id])
    REFERENCES [dbo].[FPCs]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_FPCJob'
CREATE INDEX [IX_FK_FPCJob]
ON [dbo].[Jobs]
    ([FPC_Id]);
GO

-- Creating foreign key on [Block_Id] in table 'Tasks'
ALTER TABLE [dbo].[Tasks]
ADD CONSTRAINT [FK_BlockTask]
    FOREIGN KEY ([Block_Id])
    REFERENCES [dbo].[Blocks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_BlockTask'
CREATE INDEX [IX_FK_BlockTask]
ON [dbo].[Tasks]
    ([Block_Id]);
GO

-- Creating foreign key on [Job_Id] in table 'Blocks'
ALTER TABLE [dbo].[Blocks]
ADD CONSTRAINT [FK_JobBlock]
    FOREIGN KEY ([Job_Id])
    REFERENCES [dbo].[Jobs]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_JobBlock'
CREATE INDEX [IX_FK_JobBlock]
ON [dbo].[Blocks]
    ([Job_Id]);
GO

-- Creating foreign key on [Station_Id] in table 'Costs'
ALTER TABLE [dbo].[Costs]
ADD CONSTRAINT [FK_StationCost]
    FOREIGN KEY ([Station_Id])
    REFERENCES [dbo].[Stations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StationCost'
CREATE INDEX [IX_FK_StationCost]
ON [dbo].[Costs]
    ([Station_Id]);
GO

-- Creating foreign key on [Station_Id] in table 'NonProductiveTasks_Test'
ALTER TABLE [dbo].[NonProductiveTasks_Test]
ADD CONSTRAINT [FK_StationTest]
    FOREIGN KEY ([Station_Id])
    REFERENCES [dbo].[Stations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StationTest'
CREATE INDEX [IX_FK_StationTest]
ON [dbo].[NonProductiveTasks_Test]
    ([Station_Id]);
GO

-- Creating foreign key on [Process_Id] in table 'ProcessReports'
ALTER TABLE [dbo].[ProcessReports]
ADD CONSTRAINT [FK_ProcessProcessReport]
    FOREIGN KEY ([Process_Id])
    REFERENCES [dbo].[Process]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProcessProcessReport'
CREATE INDEX [IX_FK_ProcessProcessReport]
ON [dbo].[ProcessReports]
    ([Process_Id]);
GO

-- Creating foreign key on [Station_Id] in table 'StateStations'
ALTER TABLE [dbo].[StateStations]
ADD CONSTRAINT [FK_StationStateStation]
    FOREIGN KEY ([Station_Id])
    REFERENCES [dbo].[Stations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StationStateStation'
CREATE INDEX [IX_FK_StationStateStation]
ON [dbo].[StateStations]
    ([Station_Id]);
GO

-- Creating foreign key on [Education_Id] in table 'Blocks'
ALTER TABLE [dbo].[Blocks]
ADD CONSTRAINT [FK_EducationBlock]
    FOREIGN KEY ([Education_Id])
    REFERENCES [dbo].[NonProductiveTasks_Education]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_EducationBlock'
CREATE INDEX [IX_FK_EducationBlock]
ON [dbo].[Blocks]
    ([Education_Id]);
GO

-- Creating foreign key on [StateStation_Id] in table 'Blocks'
ALTER TABLE [dbo].[Blocks]
ADD CONSTRAINT [FK_StateStationBlock]
    FOREIGN KEY ([StateStation_Id])
    REFERENCES [dbo].[StateStations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StateStationBlock'
CREATE INDEX [IX_FK_StateStationBlock]
ON [dbo].[Blocks]
    ([StateStation_Id]);
GO

-- Creating foreign key on [Operator_Id] in table 'ActivitySkills'
ALTER TABLE [dbo].[ActivitySkills]
ADD CONSTRAINT [FK_OperatorActivitySkill]
    FOREIGN KEY ([Operator_Id])
    REFERENCES [dbo].[Operators]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_OperatorActivitySkill'
CREATE INDEX [IX_FK_OperatorActivitySkill]
ON [dbo].[ActivitySkills]
    ([Operator_Id]);
GO

-- Creating foreign key on [Activity_Id] in table 'ActivitySkills'
ALTER TABLE [dbo].[ActivitySkills]
ADD CONSTRAINT [FK_ActivityActivitySkill]
    FOREIGN KEY ([Activity_Id])
    REFERENCES [dbo].[Activities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivityActivitySkill'
CREATE INDEX [IX_FK_ActivityActivitySkill]
ON [dbo].[ActivitySkills]
    ([Activity_Id]);
GO

-- Creating foreign key on [ProductRework_Id] in table 'ProductActivitySkills'
ALTER TABLE [dbo].[ProductActivitySkills]
ADD CONSTRAINT [FK_ProductReworkProductActivitySkill]
    FOREIGN KEY ([ProductRework_Id])
    REFERENCES [dbo].[ProductReworks]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProductReworkProductActivitySkill'
CREATE INDEX [IX_FK_ProductReworkProductActivitySkill]
ON [dbo].[ProductActivitySkills]
    ([ProductRework_Id]);
GO

-- Creating foreign key on [ActivitySkill_Id] in table 'ProductActivitySkills'
ALTER TABLE [dbo].[ProductActivitySkills]
ADD CONSTRAINT [FK_ActivitySkillProductActivitySkill]
    FOREIGN KEY ([ActivitySkill_Id])
    REFERENCES [dbo].[ActivitySkills]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ActivitySkillProductActivitySkill'
CREATE INDEX [IX_FK_ActivitySkillProductActivitySkill]
ON [dbo].[ProductActivitySkills]
    ([ActivitySkill_Id]);
GO

-- Creating foreign key on [ProcessReport_Id] in table 'OperatorProcessReports'
ALTER TABLE [dbo].[OperatorProcessReports]
ADD CONSTRAINT [FK_ProcessReportOperatorProcessReport]
    FOREIGN KEY ([ProcessReport_Id])
    REFERENCES [dbo].[ProcessReports]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProcessReportOperatorProcessReport'
CREATE INDEX [IX_FK_ProcessReportOperatorProcessReport]
ON [dbo].[OperatorProcessReports]
    ([ProcessReport_Id]);
GO

-- Creating foreign key on [ProcessOperator_Id] in table 'OperatorProcessReports'
ALTER TABLE [dbo].[OperatorProcessReports]
ADD CONSTRAINT [FK_ProcessOperatorOperatorProcessReport]
    FOREIGN KEY ([ProcessOperator_Id])
    REFERENCES [dbo].[ProcessOperators]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ProcessOperatorOperatorProcessReport'
CREATE INDEX [IX_FK_ProcessOperatorOperatorProcessReport]
ON [dbo].[OperatorProcessReports]
    ([ProcessOperator_Id]);
GO

-- Creating foreign key on [StartSSA_Id] in table 'ExternalConnectors'
ALTER TABLE [dbo].[ExternalConnectors]
ADD CONSTRAINT [FK_StateStationActivityExternalConnector]
    FOREIGN KEY ([StartSSA_Id])
    REFERENCES [dbo].[StateStationActivities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StateStationActivityExternalConnector'
CREATE INDEX [IX_FK_StateStationActivityExternalConnector]
ON [dbo].[ExternalConnectors]
    ([StartSSA_Id]);
GO

-- Creating foreign key on [EndSSA_Id] in table 'ExternalConnectors'
ALTER TABLE [dbo].[ExternalConnectors]
ADD CONSTRAINT [FK_StateStationActivityExternalConnector1]
    FOREIGN KEY ([EndSSA_Id])
    REFERENCES [dbo].[StateStationActivities]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StateStationActivityExternalConnector1'
CREATE INDEX [IX_FK_StateStationActivityExternalConnector1]
ON [dbo].[ExternalConnectors]
    ([EndSSA_Id]);
GO

-- Creating foreign key on [Part_Id] in table 'MachineParts'
ALTER TABLE [dbo].[MachineParts]
ADD CONSTRAINT [FK_PartMachinePart]
    FOREIGN KEY ([Part_Id])
    REFERENCES [dbo].[Parts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_PartMachinePart'
CREATE INDEX [IX_FK_PartMachinePart]
ON [dbo].[MachineParts]
    ([Part_Id]);
GO

-- Creating foreign key on [Maintenance_Id] in table 'MachinePartMaintenances'
ALTER TABLE [dbo].[MachinePartMaintenances]
ADD CONSTRAINT [FK_MaintenanceMachinePartMaintenance]
    FOREIGN KEY ([Maintenance_Id])
    REFERENCES [dbo].[Maintenances]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MaintenanceMachinePartMaintenance'
CREATE INDEX [IX_FK_MaintenanceMachinePartMaintenance]
ON [dbo].[MachinePartMaintenances]
    ([Maintenance_Id]);
GO

-- Creating foreign key on [MachinePart_Id] in table 'MachinePartMaintenances'
ALTER TABLE [dbo].[MachinePartMaintenances]
ADD CONSTRAINT [FK_MachinePartMachinePartMaintenance]
    FOREIGN KEY ([MachinePart_Id])
    REFERENCES [dbo].[MachineParts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MachinePartMachinePartMaintenance'
CREATE INDEX [IX_FK_MachinePartMachinePartMaintenance]
ON [dbo].[MachinePartMaintenances]
    ([MachinePart_Id]);
GO

-- Creating foreign key on [MachinePartMaintenance_Id] in table 'MaintenanceReports'
ALTER TABLE [dbo].[MaintenanceReports]
ADD CONSTRAINT [FK_MachinePartMaintenanceMaintenanceReport]
    FOREIGN KEY ([MachinePartMaintenance_Id])
    REFERENCES [dbo].[MachinePartMaintenances]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MachinePartMaintenanceMaintenanceReport'
CREATE INDEX [IX_FK_MachinePartMaintenanceMaintenanceReport]
ON [dbo].[MaintenanceReports]
    ([MachinePartMaintenance_Id]);
GO

-- Creating foreign key on [Machine_Id] in table 'MachineParts'
ALTER TABLE [dbo].[MachineParts]
ADD CONSTRAINT [FK_MachineMachinePart]
    FOREIGN KEY ([Machine_Id])
    REFERENCES [dbo].[Machines]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MachineMachinePart'
CREATE INDEX [IX_FK_MachineMachinePart]
ON [dbo].[MachineParts]
    ([Machine_Id]);
GO

-- Creating foreign key on [StoppageReport_Id] in table 'Repairs'
ALTER TABLE [dbo].[Repairs]
ADD CONSTRAINT [FK_StoppageReportRepair]
    FOREIGN KEY ([StoppageReport_Id])
    REFERENCES [dbo].[StoppageReports]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_StoppageReportRepair'
CREATE INDEX [IX_FK_StoppageReportRepair]
ON [dbo].[Repairs]
    ([StoppageReport_Id]);
GO

-- Creating foreign key on [MachinePart_Id] in table 'Repairs'
ALTER TABLE [dbo].[Repairs]
ADD CONSTRAINT [FK_MachinePartRepair]
    FOREIGN KEY ([MachinePart_Id])
    REFERENCES [dbo].[MachineParts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_MachinePartRepair'
CREATE INDEX [IX_FK_MachinePartRepair]
ON [dbo].[Repairs]
    ([MachinePart_Id]);
GO

-- Creating foreign key on [Id] in table 'NonProductiveTasks_PM'
ALTER TABLE [dbo].[NonProductiveTasks_PM]
ADD CONSTRAINT [FK_PM_inherits_NonProductiveTask]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[NonProductiveTasks]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'NonProductiveTasks_Education'
ALTER TABLE [dbo].[NonProductiveTasks_Education]
ADD CONSTRAINT [FK_Education_inherits_NonProductiveTask]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[NonProductiveTasks]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'NonProductiveTasks_Setup'
ALTER TABLE [dbo].[NonProductiveTasks_Setup]
ADD CONSTRAINT [FK_Setup_inherits_NonProductiveTask]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[NonProductiveTasks]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Id] in table 'NonProductiveTasks_Test'
ALTER TABLE [dbo].[NonProductiveTasks_Test]
ADD CONSTRAINT [FK_Test_inherits_NonProductiveTask]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[NonProductiveTasks]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
























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
		(47,'53','txtAbout',37);



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
(43,1,0,'2013-01-01',31),
(44,1,0,'2013-01-01',31),
(45,1,0,'2013-01-01',31),
(46,1,0,'2013-01-01',31);


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
(9, N'حسین عابدی', '009', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(10, N'رضا کیانی', '011', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(11, N'محسن حسینقلی', '021', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(12, N'سید فراز ذاکر زاده', '102', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(13, N'مهری ماهری', '103', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(14, N'غلامحسین مقصودی', '204', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(15, N'هوشنگ نهایت', '015', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(16, N'مائده صفرپور', '106', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(17, N'سعید سعیدی', '237', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'False'),
(18, N'حمید حمیدی', '128', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(19, N'مجید مجیدی', '328', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True'),
(20, N'فرید فریدی', '458', 0, {fn CURRENT_TIMESTAMP()},{fn CURRENT_TIMESTAMP()}, 1, 1, 20, 'True');
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
insert into dbo.StateStationActivities (Id, CycleTime, ManHour, StateStation_Id, Activity_Id, IsMany, IsInJob) values 
(1,60,2,1,1, 'True', 'True'),
(2,90,1,1,2, 'False', 'False'),
(3,60,2,2,1, 'True', 'True'),
(4,85,1,2,2, 'False', 'False'),
(5,40,1,3,3, 'True', 'False'),
(6,50,1,3,4, 'False', 'False'),
(7,60,1,3,5, 'True', 'False'),
(8,60,3,4,3, 'True', 'True'),
(9,110,1,4,4, 'True', 'False'),
(10,60,1,5,2, 'True', 'True'),
(11,60,1,6,1, 'True', 'True'),
(12,60,1,7,1, 'False', 'True'),
(13,60,1,8,1, 'True', 'True'),
(14,60,1,9,1, 'True', 'True'),
(15,60,1,10,2, 'True', 'True'),
(16,60,1,11,1, 'True', 'True'),
(17,60,1,12,2, 'False', 'True'),
(18,60,1,13,1, 'True', 'True'),
(19,60,1,14,1, 'True', 'True'),
(20,60,1,21,2, 'True', 'True'),
(21,60,1,15,2, 'False', 'True'),
(22,60,1,16,1, 'True', 'True'),
(23,60,1,16,2, 'True', 'False'),
(24,60,1,16,5, 'True', 'False'),
(25,60,1,17,2, 'False', 'True'),
(26,60,1,17,3, 'True', 'False'),
(27,60,1,17,4, 'False', 'False'),
(28,60,1,18,4, 'True', 'True'),
(29,60,1,18,2, 'True', 'False'),
(30,60,1,19,3, 'True', 'True'),
(31,60,1,19,1, 'True', 'False'),
(32,60,1,20,1, 'False', 'True'),
(33,60,1,20,2, 'True', 'False'),
(34,60,1,22,3, 'False', 'True'),
(35,60,1,22,1, 'True', 'False'),
(36,60,1,23,4, 'False', 'True'),
(37,60,1,23,1, 'False', 'False'),
(38,60,1,24,3, 'True', 'True'),
(39,60,1,25,4, 'True', 'True'),
(40,60,1,26,3, 'True', 'True'),
(41,60,1,27,4, 'False', 'True'),
(42,60,1,28,1, 'True', 'True'),
(43,60,1,29,1, 'True', 'True'),
(44,60,1,30,3, 'True', 'True'),
(45,60,1,31,1, 'True', 'True'),
(46,60,1,32,1, 'True', 'True'),
(47,60,1,33,3, 'False', 'True'),
(48,333,1,34,1, 'True', 'True'),
(49,222,1,35,1, 'True', 'True'),
(50,60,1,36,1, 'True', 'True'),
(51,111,1,36,2, 'True', 'True'),
(52,60,1,37,1, 'True', 'True'),
(53,60,1,39,2, 'False', 'True'),
(54,60,1,40,1, 'True', 'True'),
(55,60,1,41,2, 'False', 'True'),
(56,60,1,42,3, 'False', 'True'),
(57,60,1,43,3, 'False', 'True'),
(58,60,1,44,1, 'True', 'True'),
(59,60,1,45,5, 'True', 'True'),
(60,60,1,46,1, 'True', 'True'),
(61,60,1,47,3, 'True', 'True'),
(62,60,1,48,1, 'True', 'True'),
(63,60,1,48,2, 'True', 'False'),
(64,120,1,49,2, 'True', 'True'),
(65,60,1,50,1, 'True', 'True'),
(66,60,1,51,3, 'True', 'True'),
(67,60,1,52,4, 'True', 'True'),
(68,60,1,53,3, 'True', 'True'),
(69,60,1,54,4, 'True', 'True'),
(70,60,1,55,3, 'True', 'True'),
(71,60,1,56,4, 'True', 'True'),
(72,60,1,60,1, 'True', 'True'),
(73,60,1,61,3, 'True', 'True'),
(74,60,1,64,1, 'True', 'True'),
(75,60,1,65,2, 'True', 'True'),
(76,60,1,67,1, 'True', 'True'),
(77,60,1,68,1, 'True', 'True'),
(78,60,2,69,3, 'True', 'True'),
(79,85,8,69,4, 'False', 'True'),
(80,45,3,1,1, 'True', 'True'),
(81,30,4,1,1, 'True', 'False'),
(82,60,2,1,2, 'True', 'False'),
(83,40,3,1,2, 'True', 'False'),
(84,44,3,2,1, 'True', 'True'),
(85,22,4,2,1, 'True', 'False'),
(86,99,1,2,1, 'True', 'False'),
(87,75,1,1,1, 'True', 'True');
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
('8','2',N'اپراتور','0', '3', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('9','2',N'سایر','1', '3', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
--سایر
('10','2',N'مونتاژ','0', '4', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('11','2',N'سایر مشکلات','1', '4', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('12','2',N'نامشخص','2', '4', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

--فنی
('13','3',N'قطعی برق','0', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('14','3',N'سیم کشی','1', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('15','3',N'قطع و وصل برق','2', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('16','3',N'نوسانات برق','3', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('17','3',N'برق گرفتگی','4', '5', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('18','3',N'خرابی دستگاه','0', '6', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('19','3',N'تنظیمات نادرست','1', '6', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('20','3',N'روغنکاری','2', '6', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('21','3',N'پنچری','3', '6', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('22','3',N'مواد درجه 2','0', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('23','3',N'تاریخ مصرف','1', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('24','3',N'ناخالصی','2', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('25','3',N'سایر','0', '7', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

--پرسنلی
('26','3',N'مهارت ناکافی','0', '8', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('27','3',N'خواب آلودگی','1', '8', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('28','3',N'سایر','2', '8', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('29','3',N'کمبود پرسنل','0', '9', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('30','3',N'آموزش نادرست','1', '9', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('31','3',N'جابجایی','2', '9', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),

('32','3',N'عدم تطبیق','0', '10', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('33','3',N'درجه 2','1', '10', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('34','3',N'مشکلات مواد','10', '11', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('35','3',N'مشکلات ایستگاه','11', '11', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('36','3',N'مشکلات ماشین','12', '11', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1),
('37','3',N'نامشخص','20', '12', {fn CURRENT_TIMESTAMP()}, {fn CURRENT_TIMESTAMP()}, 1, 1);
SET IDENTITY_INSERT dbo.Causes OFF
