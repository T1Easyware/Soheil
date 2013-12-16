using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.DataServices;
using Soheil.Model;

namespace Soheil.Core.ViewModels
{
    public class UserAccessNodeVM : AccessNodeViewModel
    {
        public static readonly DependencyProperty UserAccessProperty =
            DependencyProperty.Register("UserAccess", typeof(AccessType), typeof(UserAccessNodeVM), new PropertyMetadata(AccessType.None));

        public AccessType UserAccess
        {
            get { return (AccessType)GetValue(UserAccessProperty); }
            set { SetValue(UserAccessProperty, value); }
        }

        public static readonly DependencyProperty PositionAccessProperty =
            DependencyProperty.Register("PositionAccess", typeof(AccessType), typeof(UserAccessNodeVM), new PropertyMetadata(AccessType.None));

        public AccessType PositionAccess
        {
            get { return (AccessType)GetValue(PositionAccessProperty); }
            set { SetValue(PositionAccessProperty, value); }
        }

        public UserAccessNodeVM(int accessRuleId, int userId, AccessRuleDataService accessRuleDataService, UserAccessRuleDataService userAccessRuleDataService, List<Tuple<int, AccessType>> ruleAccessList, AccessType access)
            : base(access)
        {
            var accessRule = accessRuleDataService.GetSingle(accessRuleId);
            Title = Common.Properties.Resources.ResourceManager.GetString(accessRule.Name);
            Id = accessRule.Id;
            ParentId = accessRule.Parent != null ? accessRule.Parent.Id : -1;
            foreach (var child in accessRule.Children)
            {
                ChildNodes.Add(new UserAccessNodeVM(child.Id, userId, accessRuleDataService, userAccessRuleDataService, ruleAccessList, Access));
            }

            var userAccessRule = userAccessRuleDataService.GetSingle(userId, accessRuleId);
            if (userAccessRule != null)
            {
                if (userAccessRule.Type != null) UserAccess = (AccessType)userAccessRule.Type;
            }

            if(!ruleAccessList.Any()) return;
            foreach (Tuple<int, AccessType> tuple in ruleAccessList.Where(item => item.Item1 == accessRuleId))
            {
                PositionAccess |= tuple.Item2;
            }
        }

        public UserAccessNodeVM( AccessType access)
            : base(access)
        {
        }
    }
}