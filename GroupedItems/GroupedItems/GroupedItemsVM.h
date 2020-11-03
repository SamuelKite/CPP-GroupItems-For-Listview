#pragma once
#include "VMBase.h"
namespace GroupedItems
{
    public ref class TitledGroup sealed : VMBase, Windows::UI::Xaml::Interop::IBindableObservableVector
    {
    public:
        TitledGroup(Platform::String^ title, Windows::Foundation::Collections::IObservableVector<Platform::String^>^ item);
        virtual ~TitledGroup();

        property Windows::Foundation::Collections::IObservableVector<Platform::String^>^ Items
        {
            Windows::Foundation::Collections::IObservableVector<Platform::String^>^ get();
        }

        property Platform::String^ Title
        {
            Platform::String^ get();
            void set(Platform::String^ val);
        }

#pragma region IBindableObservableVector_Implementation
        virtual Windows::UI::Xaml::Interop::IBindableIterator^ First();
        virtual Platform::Object^ GetAt(unsigned int index);
        virtual Windows::UI::Xaml::Interop::IBindableVectorView^ GetView();
        virtual bool IndexOf(Platform::Object^ value, unsigned int* index);
        virtual void SetAt(unsigned int index, Platform::Object^ value);
        virtual void InsertAt(unsigned int index, Platform::Object^ value);
        virtual void RemoveAt(unsigned int index);
        virtual void Append(Platform::Object^ value);
        virtual void RemoveAtEnd();
        virtual void Clear();
        virtual event Windows::UI::Xaml::Interop::BindableVectorChangedEventHandler^ VectorChanged;
        virtual property unsigned int Size
        {
            unsigned int get();
        }
#pragma endregion IBindableObservableVector_Implementation

    private:
        void TitledGroup::OnCollectionChanged(Windows::Foundation::Collections::IObservableVector<Platform::String^>^ sender, Windows::Foundation::Collections::IVectorChangedEventArgs^ args);
        Windows::Foundation::Collections::IObservableVector<Platform::String^>^ items;
        Platform::String^ title;
        Windows::Foundation::EventRegistrationToken vectorChangedToken;
    };

    public ref class GroupedItemsVM sealed : VMBase
    {
    public:
        GroupedItemsVM();

        property Windows::Foundation::Collections::IObservableVector<TitledGroup^>^ TitledGroups
        {
            Windows::Foundation::Collections::IObservableVector<TitledGroup^>^ get();
        }

    private:
        Windows::Foundation::Collections::IObservableVector<TitledGroup^>^ titledGroups;
    };
}

