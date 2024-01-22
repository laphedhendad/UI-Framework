# UI Framework

Фреймворк для создания гибкого интерфейса на движке Unity, основанный на реактивности и паттерне Model-View-Presenter.

## Содержание

  - [Установка](#установка)
  - [Реактивность](#реактивность)
  - [Model-View-Presenter](#model-view-presenter)
    - [Model](#model)
    - [View](#view)
    - [Presenter](#presenter)
    - [Простой Presenter](#простой-presenter)
  - [Архитектура UI](#архитектура-ui)
  - [Обучающий проект](#обучающий-проект)

# Установка

$${\color{orange}ВАЖНО!}$$

Фреймворк использует в качестве зависимости другой пакет [Lightweight Reactivity Package](https://github.com/laphedhendad/com.laphed.reactivity). Установите его перед установкой UI Framework.
<hr>

  * Через .unitypackage файл:

    Скачайте пакет со [страницы релизов](https://github.com/laphedhendad/com.laphed.ui-framework/releases).
    Добавьте его в свой проект с помощью Assets/Import Package/Custom Package...
  * Через ссылку на git:

    Откройте Window/Package Manager, выберите +/Add package from git URL...
    
    Укажите ссылку на пакет: `https://github.com/laphedhendad/com.laphed.ui-framework.git`
    
    Можно выбрать конкретную версию пакета, указав её в ссылке: `https://github.com/laphedhendad/com.laphed.ui-framework.git#1.0.0`

# Реактивность

Взаимодействие интерфейса и данных построено на реактивном подходе. В пакете реализовано реактивное свойство, коллекция и словарь. Они представляют собой облегченные версии соответствующих классов из плагина [UniRx](https://github.com/neuecc/UniRx).

## ReactiveProperty

Реактивное свойство - простая обертка над типами данных. Вызывает событие OnChanged, когда меняется его значение.

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

Реактивная коллекция - обертка над типом [Collection](https://learn.microsoft.com/ru-ru/dotnet/api/system.collections.objectmodel.collection-1). Вызывает события:

* OnChanged
* OnAdd
* OnRemove
* OnReplace
* OnCleared

В параметрах событий передает индекс элемента, подвергшегося изменениям.

## ReactiveDictionary

Реактивный словарь - обертка над типом [Dictionary](https://learn.microsoft.com/ru-ru/dotnet/api/system.collections.generic.dictionary-2). Вызывает события:

* OnChanged
* OnAdd
* OnRemove
* OnReplace
* OnCleared

В параметрах событий передает ключ элемента, подвергшегося изменениям.

## Пример использования

```csharp
  //класс бустера с реактивным свойством
  public class Booster: IBooster
  {
      public IReactiveProperty<int> Amount { get; } = new ReactiveProperty<int>();
  }

  ...

  //подписка на реактивное свойство
  public override void SubscribeModel(TReactive model)
  {
      if (model == null) return;
      this.model = model;
      model.OnChanged += HandleModelUpdate;
  }

  ...

  //изменение значения реактивного свойства
  private void BuyBooster()
  {
      booster.Amount.Value++;
  }
```

# Model-View-Presenter

Паттерн MVP предполагает разбитие связи между Model(модель данных) и View(отображение) с помощью дополнительного класса Presenter(представитель).

Presenter:
  * Подписывается на события модели и может напрямую менять данные
  * Хранит ссылку на View и обновляет его через интерфейс
  * Подписывается на события View и обрабатывает их
  * Имеет одинаковое с View время жизни и связан только с одним отображением
    
![MVP](https://github.com/laphedhendad/com.laphed.ui-framework/assets/52206303/bae3b5fe-b01c-496d-9ecf-6b93770fee03)

## Model

В роли модели выступает реактивное свойство/коллекция/словарь. View реализует интерфейс IView:

```csharp
  public interface IView<T>
  {
      event Action OnDispose;
      void UpdateView(T value);
  }
```

## View

Для MonoBehaviour отображений в пакете присутствует абстрактный класс MonoView и его наследники для конкретных типов данных:

```csharp
  //базовый класс для MonoBehaviour отображений
  public abstract class MonoView<T>: MonoBehaviour, IView<T>
  {
      public event Action OnDispose;
      public abstract void UpdateView(T value);
      protected virtual void OnDestroy() => OnDispose?.Invoke();
  }

  //MonoView для конкретного типа данных
  public abstract class MonoTextView: MonoView<string>
  {
      protected abstract string Text { set; }
      public override void UpdateView(string value) => Text = value;
  }

  //конкретная реализация View
  public class SpeakerNameView : MonoTextView
  {
      [SerializeField] private TMP_Text speakerNameText;
      protected override string Text
      {
          set => speakerNameText.text = value;
      }
  }
```

Вы можете сами создавать MonoView для собственных типов данных. Существующие в пакете решения покрывают только самые частые типы.

## Presenter

Presenter реализует интерфейс IPresenter:

```csharp
  public interface IPresenter<TModel>: IDisposable
  {
      void SubscribeModel(TModel model);
  }
```

Взаимодействие с разными реактивными моделями предполагает использование соответствующего представителя. В пакете представлены:

* PropertryPresenter<TModelData, TViewData> для свойств
* CollectionPresenter<TModelData, TViewData> для коллекций
* DictionaryPresenter<TKey, TValue, TViewData> для словарей

Для каждого из вышеперечисленных классов существует парный класс для ситуаций, в которых не требуется конвертация типов данных для отображения.

* PropertyPresenter<TData>
* CollectionPresenter<TData>
* DictionaryPresenter<TKey, TValue>
 
Создание собственного Presenter'а предполагает наследование от этих 6 классов и переопределение методов SubscribeModel и HandleModelUpdate при необходимости.

```csharp
  //пример представителя для отображения количества бустеров
  //и обработки нажатия на кнопку бустера
  public class BoosterPresenter: PropertyPresenter<int>
  {
      //конкретизируем тип данных отображения
      //для обработки событий пользовательского ввода
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

  //подписка презентера на модель
  public override void Bind()
  {
      boosterPresenter = new BoosterPresenter(boosterButton, buyBoosterWindow);
      boosterPresenter.SubscribeModel(booster.Amount);
  }
```

## Простой Presenter

Часто возникают ситуации, когда отображению необходимо только показывать значение модели без лишней логики. В таких кейсах создаются экземпляры классов PropertyPresenter<TData>, CollectionPresenter<TData>, DictionaryPresenter<TKey, TValue>.

```csharp
  public void Bind()
  {
      //создание PropertyPresenter<TData> без наследования
      var speakerNamePresenter = new PropertyPresenter<string>(speakerNameView);

      //подписка представителя на модель
      speakerNamePresenter.SubscribeModel(reactivePassage.SpeakerName);
  }
```

# Архитектура UI

Весь UI в пакете делится на Окна (Window) и элементы. Окно - это контейнер для элементов (включая вложенные окна). Окном может называться как весь экран приложения, так и отдельное модальное окно. У окон только две ответственности: определять порядок инициализации binder'ов (см. ниже) и вызывать очистку ресурсов при уничтожении.

UI связывается с остальным кодом приложения через binder. В binder прокидываются ссылки на компоненты и описывается их взаимодействие с дочерними ему элементами UI. Каждому окну должен соответствовать свой binder, при этом binder может ссылаться на элементы из окон, находящихся в иерархии на любой позиции относительно его окна.

Создание окна предполагает создание GameObject'а с компонентом MonoWindow и конкретной реализацией MonoBinder.

![MonoWindow](https://github.com/laphedhendad/com.laphed.ui-framework/assets/52206303/2e084ed9-16e5-4c00-97b6-ce4eb7d77449)

В компонент MonoWindow прокидываются ссылки на его дочерние окна. Делать это руками не обязательно. Достаточно перед тестом приложения нажать кнопку "Find subwindows automatically" на корневом компоненте MonoWindow. Все окна в иерархии сами установят ссылки на дочерние окна.

```csharp
  //пример реализации Binder'а для окна использования бустера
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

# Обучающий проект

[Обучающий проект](https://github.com/laphedhendad/UI-Framework-Example) можно скачать архивом с GitHub. Проект использует [Zenject](https://github.com/modesttree/Zenject) и [UniTask](https://github.com/Cysharp/UniTask) в достаточно небольшом количестве, чтобы не отрывать внимание от пакета.
