#include "pch.h"
#include "VMBase.h"

namespace GroupedItems 
{
    concurrency::task<void> VMBase::RaisePropertyChanged(Platform::String^ property)
    {
        Platform::WeakReference weak(this);

        return concurrency::create_task(Dispatcher->RunAsync(Windows::UI::Core::CoreDispatcherPriority::Normal,
        ref new Windows::UI::Core::DispatchedHandler([weak, property]()
        {
            try 
            {
                auto vm = weak.Resolve<VMBase>();
                if (vm != nullptr)
                {
                    vm->PropertyChanged(vm, ref new Windows::UI::Xaml::Data::PropertyChangedEventArgs{ property });
                }
            }
            catch (Platform::Exception^ ex)
            {
                DebugBreak();
            }
            catch (std::exception const& ex)
            {
                DebugBreak();
            }
        })));
    }
}
