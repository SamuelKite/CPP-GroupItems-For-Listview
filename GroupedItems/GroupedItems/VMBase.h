#pragma once
namespace GroupedItems
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class VMBase : Windows::UI::Xaml::DependencyObject, Windows::UI::Xaml::Data::INotifyPropertyChanged
    {
    public:
        virtual event Windows::UI::Xaml::Data::PropertyChangedEventHandler^ PropertyChanged;

    internal:
        concurrency::task<void> RaisePropertyChanged(Platform::String^ propertyName);
        
        template<typename T>
        inline bool SetProperty(T& last, T& next, Platform::String^ property)
        {
            auto changed = last != next;
            if (changed)
            {
                last = next;
                RaisePropertyChanged(property);
            }

            return changed;
        }
    };

}