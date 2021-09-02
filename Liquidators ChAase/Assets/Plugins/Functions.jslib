var plugin = {
    loginReq: function () {
		login();
	  },

	logOutReq: function () {
		logOut();
	},
};

mergeInto(LibraryManager.library, plugin);