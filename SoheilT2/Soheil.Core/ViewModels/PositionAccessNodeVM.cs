using System.Windows;
using Soheil.Common;
using Soheil.Core.Base;
using Soheil.Core.DataServices;

namespace Soheil.Core.ViewModels
{
    public class PositionAccessNodeVM : AccessNodeViewModel
    {
        public static readonly DependencyProperty PositionAccessProperty =
            DependencyProperty.Register("PositionAccess", typeof(AccessType), typeof(PositionAccessNodeVM), new PropertyMetadata(AccessType.None));

        public AccessType PositionAccess
        {
            get { return (AccessType)GetValue(PositionAccessProperty); }
            set { SetValue(PositionAccessProperty, value);}
        }

        public PositionAccessNodeVM(int accessRuleId, int positionId, AccessRuleDataService accessRuleDataService, PositionAccessRuleDataService positionAccessRuleDataService, AccessType access)
            : base(access)
        {
            var accessRule = accessRuleDataService.GetSingle(accessRuleId);
            Title = accessRule.Name;
            Id = accessRule.Id;
            ParentId =accessRule.Parent != null? accessRule.Parent.Id : -1;
            foreach (var child in accessRule.Children)
            {
                ChildNodes.Add(new PositionAccessNodeVM(child.Id,positionId,accessRuleDataService, positionAccessRuleDataService, Access));
            }

            var positionAccessRule = positionAccessRuleDataService.GetSingle(positionId, accessRuleId);
            if (positionAccessRule != null)
            {
                if (positionAccessRule.Type != null) 
                    PositionAccess = (AccessType) positionAccessRule.Type;
            }
        }

        public PositionAccessNodeVM(AccessType access):base(access)
        {
        }


    }
}