# Использование
Приложение запускается из консоли со следующим форматом 

`%appname% command [prop1 [prop2 [prop3]]] img1 img2 [progress]`

## Первая часть задания

* mirror {x|y} 
Отражение по горизонтали или по вертикали, в зависомсти от указанного параметра

* rotate {cw|ccw} (angle) 
Поворот по или против часовой стрелки на заданное количество градусов, например: rotate cw 90

* sobel {rep|odd|even} {x|y} 
Фильтр Собеля, обнаруживающий горизонтальные или вертикальные контуры. 
Первый параметр отвечает за способ экстраполяции изображений

* median (rad) 
Медианная фильтрация, параметр rad — целочисленный радиус фильтра, 
то есть размер фильтра — квадрат со стороной (2 * rad + 1)

* gauss {rep|odd|even} (sigma) 
Фильтр Гаусса, параметр sigma — вещественный параметр фильтра

* gradient {rep|odd|even} (sigma) 
Модуль градиента

## Вторая часть задания

* mse	 	
Вычисление метрики MSE, на вход подаётся два изображения, результат выводится в консоль

* psnr	 	
Вычисление метрики PSNR

* ssim	 	
Вычисление метрики SSIM

* mssim	 	
Вычисление метрики MSSIM по блокам 8x8

* dir (sigma)	 	
Визуализация направлений градиента

* nonmax (sigma)	 	
Результат немаксимального подавления

* canny (sigma) (thr_high) (thr_low)	 	
Детектирование границ с помощью алгоритма Канни. Первый параметр — сигма для вычисления частных производных, следующие два параметра - больший и меньший пороги соответственно в тысячных долях (целое число в диапазоне от 0 до 1000)
