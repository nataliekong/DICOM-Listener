   M S B A M L   `   `   `  ���� H  DDICOMListener, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:DICOMListenerDICOMListener      DICOMListener.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:DICOMListener       � Title$  DICOM Listener��$��540.4��$	��800��$y�Hidden=�.���  � ColumnDefinitions �� $
��251* ��� $
��542* �.+��� -    ��Name$ ModeGroupBox��$
��Mode��$
��Left=�$��60��$��	10,10,0,0q�$	��Top=�$	��571��$��2��.��	� -    	�Name$ WatchDirectoryRadioButton��$��Watch Directory��$
��Left=�$��27��$��	19,10,0,0q�$	��Top=��� -    ��Name$ IncludeSubfoldersCheckBox��$��Include Subfolders��$
��Left=�$��
6.8,34,0,0q�$	��Top=�$��27��$��1���� -    ��Name$ SelectDirectoryLabel��$��Select Directory��$
��Left=�$��	10,75,0,0q�$	��Top=� J� RenderTransformOrigin$ 0.078,-0.047&�$	��176��$
��Bold"��� -    ��Name$ SelectDirectoryTextBox��$
��Left=�$��23��$��
10,106,0,0q� �� TextWrapping$
 Wrap=�$	��Top=�$	��571��$��2������ 	 �� XPath$	 DirectoryPath��
 �� UpdateSourceTrigger$
 PropertyChanged=��� -    ��Name$ BrowseButton��$��Browse��$
��Left=�$��343.8,106,0,0q�$	��Top=�$��85��$ -0.387,-0.979&�$��23��$��1���� -   $ StartWatchingButton��$��Start Watching��$
��Left=�$��
10,143,0,0q�$	��Top=�$	��126���� -   $ WatchingLogLabel��$��Watching Log��$
��Left=�$��
10,188,0,0q�$	��Top=�$ 0.078,-0.047&�$	��176��$
��Bold"�$y�Hidden=��� -	   $ WatchingLogTextBox��$
��Left=�$	��264��$��
10,219,0,0q�$
 Wrap=� �� AcceptsReturn . " �� VerticalScrollBarVisibility$ Visible=�$	��Top=�$	��760��$��2��$y�Hidden=��� -
   $ button��$��Button��$��1��$
��Left=�$��114.8,162,0,0q�$	��Top=�$��75��$y�Hidden=�