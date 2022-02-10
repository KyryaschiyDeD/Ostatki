using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Остатки.Classes;

namespace Остатки.Pages.SettingPages
{
	public sealed partial class HostsPage : Page
	{
		public HostsPage()
		{
			this.InitializeComponent();
			GoToHostsList();
		}

		public ObservableCollection<Hosts> HostsList = new ObservableCollection<Hosts>();

		private void GoToHostsList()
		{
			HostsList = new ObservableCollection<Hosts>(Global.WebHosting);
			CountOfshops.Text = $"Количество хостов: {HostsList.Count}";
		}

		private void CreateNewHost_Click(object sender, RoutedEventArgs e)
		{
			Hosts host = new Hosts();
			host.Link = LinkNew.Text;
			host.TimeCreate = DateTime.Now;
			WebHostingsJob.AddNewHosts(host);
			Zeroing();
			GoToHostsList();
			this.InitializeComponent();
		}

		private void Zeroing()
		{
			LinkNew.Text = "";
		}
	}
}
