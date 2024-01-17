# UI Framework
[README RU](https://github.com/laphedhendad/com.laphed.ui-framework/blob/main/README_RU.md)

UI Framework for Unity based on reactivity and Model-View-Presenter pattern.

## Table of Contents

- [Installation](#installation)
- [Reactivity](#reactivity)
- [Model-View-Presenter](#model-view-presenter)
  - [Model](#model)
  - [View](#view)
  - [Presenter](#presenter)
  - [Simple Presenter](#simple-presenter)
- [UI Architecture](#ui-architecture)
- [Example Project](#example-project)

# Installation

  * Using .unitypackage file:

    Download package from [releases page](https://github.com/laphedhendad/com.laphed.ui-framework/releases).
    Add to project with Assets/Import Package/Custom Package...
    
  * Via git URL:

    Open Window/Package Manager and choose +/Add package from git URL...
    
    Set `https://github.com/laphedhendad/com.laphed.ui-framework.git` as URL.

    If you want to set a target version, UI Framework uses the *.*.* release tag so you can specify a version like #1.0.0. For example `https://github.com/laphedhendad/com.laphed.ui-framework.git#1.0.0`.

# Reactivity

Взаимодействие интерфейса и данных построено на реактивном подходе. В пакете реализовано реактивное свойство, коллекция и словарь. Они представляют собой облегченные версии соответствующих классов из плагина [UniRx](https://github.com/neuecc/UniRx).
The interaction between interface and data is built on a reactive approach. The package implements reactive property, collection and dictionary. They are lightweight versions of the corresponding classes from the [UniRx](https://github.com/neuecc/UniRx) plugin.

## ReactiveProperty

Reactive Property - a simple wrapper around data types. Triggers the OnChanged event when its value changes.

```csharp
  public class ReactiveProperty<T>: IReactiveProperty<T>
  {
      public event Action OnChanged;
      private T currentValue;

      public T Value
      {
          get => currentValue;
          set
          {
              if (EqualityComparer<T>.Default.Equals(value, currentValue)) return;
              currentValue = value;
              OnChanged?.Invoke();
          }
      }
  }
```

## ReactiveCollection

Reactive Collection - a wrapper around a type [Collection](https://learn.microsoft.com/ru-ru/dotnet/api/system.collections.objectmodel.collection-1). Triggers events:

* OnChanged
* OnAdd
* OnRemove
* OnReplace
* OnCleared

The events pass the index of the changed element as parameters.

## ReactiveDictionary

Reactive Dictionary - a wrapper around a type [Dictionary](https://learn.microsoft.com/ru-ru/dotnet/api/system.collections.generic.dictionary-2). Triggers events:

* OnChanged
* OnAdd
* OnRemove
* OnReplace
* OnCleared

The events pass the key of the changed element as parameters.

## Example

```csharp
  //booster class with reactive property
  public class Booster: IBooster
  {
      public IReactiveProperty<int> Amount { get; } = new ReactiveProperty<int>();
  }

  ...

  //subscribing to a reactive property
  public override void SubscribeModel(TReactive model)
  {
      if (model == null) return;
      this.model = model;
      model.OnChanged += HandleModelUpdate;
  }

  ...

  //changing value of the reactive property
  private void BuyBooster()
  {
      booster.Amount.Value++;
  }
```

# Model-View-Presenter

The MVP pattern involves breaking the connection between Model and View with the help of an additional Presenter class.

Presenter:
  * Subscribes to model events and can directly modify data
  * Holds a reference to View and updates it through the interface
  * Subscribes to View events and processes them
  * Has the same lifetime as View and is associated with only one view
    
![MVP](https://github.com/laphedhendad/com.laphed.ui-framework/assets/52206303/bae3b5fe-b01c-496d-9ecf-6b93770fee03)

## Model

Reactive property/collection/dictionary acts as the model. View implements the IView interface:

```csharp
  public interface IView<T>
  {
      event Action OnDispose;
      void UpdateView(T value);
  }
```

## View

For MonoBehaviour views in the package, there is an abstract class MonoView and its descendants for specific data types:

```csharp
  //base class for MonoBehaviour Views
  public abstract class MonoView<T>: MonoBehaviour, IView<T>
  {
      public event Action OnDispose;
      public abstract void UpdateView(T value);
      protected virtual void OnDestroy() => OnDispose?.Invoke();
  }

  //MonoView for concrete data type
  public abstract class MonoTextView: MonoView<string>
  {
      protected abstract string Text { set; }
      public override void UpdateView(string value) => Text = value;
  }

  //concrete View realization
  public class SpeakerNameView : MonoTextView
  {
      [SerializeField] private TMP_Text speakerNameText;
      protected override string Text
      {
          set => speakerNameText.text = value;
      }
  }
```

You can create MonoView for your own data types. Existing solutions in the package cover only the most common types.

## Presenter

Presenter implements the IPresenter interface:

```csharp
  public interface IPresenter<TModel>: IDisposable
  {
      void SubscribeModel(TModel model);
  }
```

Interaction with different reactive models involves using the corresponding presenter. The package includes:

* PropertryPresenter<TModelData, TViewData> for properties
* CollectionPresenter<TModelData, TViewData> for collections
* DictionaryPresenter<TKey, TValue, TViewData> for dictionaries

For each of the above classes, there is a paired class for situations where type conversion is not required for display.

* PropertyPresenter<TData>
* CollectionPresenter<TData>
* DictionaryPresenter<TKey, TValue>
 
Creating your own Presenter involves inheriting from these six classes and overriding the SubscribeModel and HandleModelUpdate methods if necessary.

```csharp
  //example presenter for displaying the number of boosters and
  //handling the booster button click
  public class BoosterPresenter: PropertyPresenter<int>
  {
      //specify data type of the View to
      //handle user input events
      private new readonly BoosterButton view;

      private readonly ModalWindowView buyBoosterWindow;

      public BoosterPresenter(BoosterButton view, ModalWindowView buyBoosterWindow) : base(view)
      {
          this.view = view;
          this.buyBoosterWindow = buyBoosterWindow;
          this.view.OnClicked += HandleClick;
      }

      private void HandleClick()
      {
          if (model == null) return;
          if (model.Value == 0)
          {
              buyBoosterWindow.Open();
              return;
          }
          model.Value--;
      }

      public override void Dispose()
      {
          view.OnClicked -= HandleClick;
          base.Dispose();
      }
  }

  ...

  //subscribing Presenter to Model
  public override void Bind()
  {
      boosterPresenter = new BoosterPresenter(boosterButton, buyBoosterWindow);
      boosterPresenter.SubscribeModel(booster.Amount);
  }
```

## Simple Presenter

There are situations where the presentation only needs to display the model's value without extra logic. In such cases, instances of PropertyPresenter<TData>, CollectionPresenter<TData>, DictionaryPresenter<TKey, TValue> are created.

```csharp
  public void Bind()
  {
      //creating of PropertyPresenter<TData> instance
      var speakerNamePresenter = new PropertyPresenter<string>(speakerNameView);

      //subscribing Presenter to Model
      speakerNamePresenter.SubscribeModel(reactivePassage.SpeakerName);
  }
```

# UI Architecture

The entire UI in the package is divided into Windows and elements. A Window is a container for elements, including nested windows. A Window can refer to either the entire application screen or a separate modal window. Windows have only two responsibilities: determining the order of binder initialization (see below) and invoking resource cleanup upon destruction.

The UI is connected to the rest of the application code through binders. References to components are passed to the binder, and their interaction with child UI elements is described. Each window should have its corresponding binder, and a binder can reference elements from windows located hierarchically at any position relative to its window.

Creating a window involves creating a GameObject with the MonoWindow component and a specific implementation of MonoBinder.

![MonoWindow](https://github.com/laphedhendad/com.laphed.ui-framework/assets/52206303/2e084ed9-16e5-4c00-97b6-ce4eb7d77449)

References to its child windows are passed to the MonoWindow component. Doing this manually is not mandatory. It is sufficient to press the "Find subwindows automatically" button on the root MonoWindow component before testing the application. All windows in the hierarchy will automatically set references to their child windows.

```csharp
  //example of Binder realization for booster activating window
  public class BoostersBinder : MonoBinder
  {
      [SerializeField] private BoosterButton boosterButton;
      [SerializeField] private ModalWindowView buyBoosterWindow;
      
      private IBooster booster;
      private BoosterPresenter boosterPresenter;
      
      [Inject]
      private void Construct(IBooster booster)
      {
          this.booster = booster;
      }
      
      public override void Bind()
      {
          boosterPresenter = new BoosterPresenter(boosterButton, buyBoosterWindow);
          boosterPresenter.SubscribeModel(booster.Amount);
      }

      protected override void Unbind()
      {
          boosterPresenter?.Dispose();
      }
  }
```

# Example Project

[The example project](https://github.com/laphedhendad/UI-Framework-Example) can be downloaded as an archive from GitHub. The project uses [Zenject](https://github.com/modesttree/Zenject) and [UniTask](https://github.com/Cysharp/UniTask) in a relatively small amount to not distract attention from the package.
