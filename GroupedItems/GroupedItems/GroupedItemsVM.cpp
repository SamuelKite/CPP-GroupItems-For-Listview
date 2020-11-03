#include "pch.h"
#include "GroupedItemsVM.h"

namespace GroupedItems
{
    TitledGroup::TitledGroup(Platform::String^ title, Windows::Foundation::Collections::IObservableVector<Platform::String^>^ items)
    {
        this->title = title;
        if (items == nullptr)
        {
            this->items = ref new Platform::Collections::Vector<Platform::String^>();
        }
        else
        {
            this->items = items;
        }

        vectorChangedToken = this->Items->VectorChanged += ref new Windows::Foundation::Collections::VectorChangedEventHandler<Platform::String^>(this, &TitledGroup::OnCollectionChanged);
    }

    TitledGroup::~TitledGroup()
    {
        this->items->VectorChanged -= vectorChangedToken;
    }

    void TitledGroup::OnCollectionChanged(Windows::Foundation::Collections::IObservableVector<Platform::String^>^ sender, Windows::Foundation::Collections::IVectorChangedEventArgs^ args)
    {
        auto vector = dynamic_cast<Windows::UI::Xaml::Interop::IBindableObservableVector^>(this);
        if (vector != nullptr)
        {
            VectorChanged(vector, args);
        }
    }

    Windows::UI::Xaml::Interop::IBindableIterator^ TitledGroup::First()
    {
        return dynamic_cast<Windows::UI::Xaml::Interop::IBindableIterator^>(this->items->First());
    }

    Platform::Object^ TitledGroup::GetAt(unsigned int index)
    {
        return this->items->GetAt(index);
    }

    Windows::UI::Xaml::Interop::IBindableVectorView^ TitledGroup::GetView()
    {
        return dynamic_cast<Windows::UI::Xaml::Interop::IBindableVectorView^>(this->items->GetView());
    }

    bool TitledGroup::IndexOf(Platform::Object^ value, unsigned int* index)
    {
        return this->items->IndexOf(dynamic_cast<Platform::String^>(value), index);
    }

    void TitledGroup::SetAt(unsigned int index, Platform::Object^ value)
    {
        this->items->SetAt(index, dynamic_cast<Platform::String^>(value));
    }

    void TitledGroup::InsertAt(unsigned int index, Platform::Object^ value)
    {
        this->items->InsertAt(index, dynamic_cast<Platform::String^>(value));
    }

    void TitledGroup::RemoveAt(unsigned int index)
    {
        this->items->RemoveAt(index);
    }

    void TitledGroup::Append(Platform::Object^ value)
    {
        this->items->Append(dynamic_cast<Platform::String^>(value));
    }

    void TitledGroup::RemoveAtEnd()
    {
        this->items->RemoveAtEnd();
    }

    void TitledGroup::Clear()
    {
        this->items->Clear();
    }

    unsigned int TitledGroup::Size::get()
    {
        return this->items->Size;
    }

    Windows::Foundation::Collections::IObservableVector<Platform::String^>^ TitledGroup::Items::get()
    {
        return this->items;
    }

    Platform::String^ TitledGroup::Title::get()
    {
        return this->title;
    }

    void TitledGroup::Title::set(Platform::String^ val)
    {
        this->title = val;
    }

    GroupedItemsVM::GroupedItemsVM() : VMBase()
    {
        auto titledGroupA = ref new Platform::Collections::Vector<Platform::String^>{ L"Some thing", L"Another" };
        auto titledGroupB = ref new Platform::Collections::Vector<Platform::String^>{ L"car", L"boat" };
        auto titledGroupC = ref new Platform::Collections::Vector<Platform::String^>{ L"velociraptor", L"fountain pen" };
        this->titledGroups = ref new Platform::Collections::Vector<TitledGroup^>();

        this->titledGroups->Append(ref new TitledGroup(L"Group A", titledGroupA));
        this->titledGroups->Append(ref new TitledGroup(L"Group B", titledGroupB));
        this->titledGroups->Append(ref new TitledGroup(L"Group C", titledGroupC));
    }

    Windows::Foundation::Collections::IObservableVector<TitledGroup^>^ GroupedItemsVM::TitledGroups::get()
    {
        return this->titledGroups;
    }
}