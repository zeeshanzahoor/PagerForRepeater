
<%@ Register TagPrefix="Senkronix" Namespace="Namespace to class" Assembly="Namespace to assembly" %>


      <asp:Repeater ID="Repeater1" runat="server">
          <ItemTemplate>
             stuff
          </ItemTemplate>
      </asp:Repeater>
      
<Senkronix:Pager ID="pgr" runat="server" PageSize="20" ControlId="Repeater1" LoadOnPageLoad="true" OnFillPager="pgr_FillPager"></Senkronix:Pager>




public class Example.cs : System.Web.UI.Page
{

        protected PagerData pgr_FillPager(object sender, PagerArgs Args)
        {
            var q = from user in UserRepository.All
                      select new UserDTO()
                      {
                          Id = user.Id,
                          Name = user.Name + " " +user.Surname,
                          Email = user.Email,
                      };
              return Args.ApplyArgs(q); // extension method is in PagerArgs class
         }
         // for updating data you need to call the following function (for example after deleting or upding record etc)
         // pgr.RefreshData();
         
         
}
