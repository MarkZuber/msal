﻿
using System;
using System.Windows.Forms;
using Microsoft.Identity.Client;

namespace DesktopTestApp
{
    public partial class MsalUserAccessTokenControl : UserControl
    {
        //private readonly TokenCache _cache;
        //private readonly MsalAccessTokenCacheItem _item;
        public delegate void RefreshView();

        public RefreshView RefreshViewDelegate { get; set; }

        // todo add id token
        //internal MsalUserAccessTokenControl(TokenCache cache, MsalAccessTokenCacheItem item) : this()
        //{
        //    _cache = cache;
        //    _item = item;
        //    accessTokenAuthorityLabel.Text = _item.Authority;
        //    accessTokenScopesLabel.Text = _item.NormalizedScopes;
        //    expiresOnLabel.Text = _item.ExpiresOn.ToString();
        //}
        
        public MsalUserAccessTokenControl()
        {
            InitializeComponent();
        }

        //private void expireAccessTokenButton_Click(object sender, System.EventArgs e)
        //{
        //    expiresOnLabel.Text = DateTimeOffset.UtcNow.ToString();
        //    _item.ExpiresOnUnixTimestamp = CoreHelpers.DateTimeToUnixTimestamp(DateTimeOffset.UtcNow);
        //    _cache.SaveAccesTokenCacheItem(_item, null);
        //}

        //private void deleteAccessTokenButton_Click(object sender, EventArgs e)
        //{
        //    var requestContext = new RequestContext(new MsalLogger(Guid.NewGuid(), null));

        //    _cache.DeleteAccessToken(_item, null, requestContext);
        //    RefreshViewDelegate?.Invoke();
        //}
    }
}
