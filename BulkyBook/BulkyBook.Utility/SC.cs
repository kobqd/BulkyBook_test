using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace BulkyBook.Utility
{
    public static class SC
    {
        public static string GetFullName(this IPrincipal user)
        {
            var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Name);
            return claim == null ? null : claim.Value;
        }
        public static string GetAddress(this IPrincipal user)
        {
            var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.StreetAddress);
            return claim == null ? null : claim.Value;
        }
    }
}
