//
// MainPage.xaml.h
// Declaration of the MainPage class.
//

#pragma once

#include "GroupedItemsVM.h"
#include "VMBase.h"
#include "MainPage.g.h"

namespace GroupedItems
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public ref class MainPage sealed
	{
	public:
		MainPage();

		property GroupedItemsVM^ VM
		{
				GroupedItemsVM^ get();
		}

	private:
			GroupedItemsVM^ vm;
			void ListView_ItemClick(Platform::Object^ sender, Windows::UI::Xaml::Controls::ItemClickEventArgs^ e);
	};
}
