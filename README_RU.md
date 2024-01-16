# UI Framework

Фреймворк для создания гибкого интерфейса на движке Unity, основанный на реактивности и паттерне Model-View-Presenter.

## Содержание

  - [Установка](#установка)
  - [Реактивность](#реактивность)
  - [Model-View-Presenter](#model-view-presenter)
  - [Архитектура UI](#фрхитектура-ui)

# Установка

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

    //подпаиска на реактивное свойство
    public BoosterPresenter(BoosterButton view, ModalWindowView buyBoosterWindow) : base(view)
    {
        this.view = view;
        this.buyBoosterWindow = buyBoosterWindow;
        this.view.OnClicked += HandleClick;
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
![Диаграмма MVP](https://github.com/laphedhendad/com.laphed.ui-framework/assets/52206303/76513671-1f7c-4832-98e0-65b39ad42e3c)

