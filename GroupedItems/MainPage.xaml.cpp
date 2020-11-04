//
// MainPage.xaml.cpp
// Implementation of the MainPage class.
//

#include "pch.h"
#include "MainPage.xaml.h"

using namespace GroupedItems;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

MainPage::MainPage()
{
    this->vm = ref new GroupedItemsVM();
    InitializeComponent();
}

GroupedItemsVM^ MainPage::VM::get()
{
    return this->vm;
}


void MainPage::ListView_ItemClick(Platform::Object^ sender, Windows::UI::Xaml::Controls::ItemClickEventArgs^ e)
{
    auto item = dynamic_cast<Platform::String^>(e->ClickedItem);
    if (item != nullptr)
    {
        Platform::String^ mover = nullptr;
        for each (TitledGroup ^ group in this->vm->TitledGroups)
        {
            unsigned int insertPoint = 0;
            if (mover != nullptr)
            {
                group->Items->Append(mover);
                mover = nullptr;
            }
            else {

                for each (Platform::String ^ string in group->Items)
                {
                    if (string == item)
                    {
                        mover = item;
                        break;
                    }
                    else 
                    {
                        insertPoint++;
                    }
                }

                if (mover != nullptr)
                {
                    group->Items->RemoveAt(insertPoint);
                }
            }
        }

        if (mover != nullptr) 
        {
            this->VM->TitledGroups->GetAt(0)->InsertAt(0, mover);
        }
    }
}
