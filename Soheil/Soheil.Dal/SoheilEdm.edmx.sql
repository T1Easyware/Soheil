
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------

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