   M S B A M L   `   `   `  ���� H  Dbruh_currency, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:bruh_currencybruh_currency      bruh_currency.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:bruh_currency     -     �Name$  ablak��$	��250��$	��500�� � WindowStartupLocation$ CenterScreen=� � 
ResizeMode$ CanMinimize=� � WindowStyle$
 None=�.���  � RowDefinitions �� -    ��Name$	 nav��$��20 � �� 	AllowDrop . ��  � ColumnDefinitions �� �� .+��� -    ��Name$
 from��$
��Left=�$��	85,84,0,0q�$	��Top=�$	��120��	 �� 
IsEditable	 . 
 �� StaysOpenOnEdit
 .  �� IsTextSearchEnabled .  $��24��$��1���� -   $ to��$��1��$
��Left=�$��	45,84,0,0q�$	��Top=�$	��120��	 . 
 .  .    �� IsTextSearchCaseSensitive .   J� RenderTransformOrigin$ 0.233,0.409&�$��24��$��1���� -    ��Name$ 
from_ertek��$
��Left=�$��24��$��	10,84,0,0q� �� TextWrapping$
 Wrap=�$�� ��$	��Top=�$��70�� �� 	MaxLength$ 7��$��1���� -   $ to_ertek��$
��Left=�$��24��$��
170,84,0,0q�$
 Wrap=�$�� ��$	��Top=�$��70��$��1��$ 0.588,1.309&�{�.  $ 9��$��1���� -    ��Name$ 	calculate��$��Átváltás��$
��Left=�$��211,183,0,0q�$	��Top=�$��75��$��20��$��2��$��1���� -   $ valuta_csere��$
��Left=�$��
230,93,0,0q�$	��Top=�$��35��$��20��$ 0.522,0.389&�$��2��$��1��.��P� ���#272727$��2�� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� �� .+��� -	    ��Name$ title��$��   Valutaváltó��
�������$��Center=�$
��Left=�$��0��$��4���� -
   $ close��$��14�����#F69B4B
�������$	��---���� -   $
 hide��$��15�����#FF6666
�������$��X��