   M S B A M L   `   `   `  ����H  DDICOMListener, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:DICOMListenerDICOMListener  5   	       DICOMListener.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:DICOMListener     5   	     � Title$  DICOM Listener��5   	   $��540.4��6    $	��800��6/   $y�Hidden=�6;   .��5	      �  � ColumnDefinitions 5
   
   �� 5      $
��251* �6   �� 5      $
��542* �6   .+�5   
   �� -   5         ��Name$ ModeGroupBox��5      $
��Mode��6)   $
��Left=�67   $��60��6R   $��	10,10,0,0q�6^   $	��Top=�6q   $	��571��6�   $��2��6�   .��5      	� -   5         	�Name$ WatchDirectoryRadioButton��5      $��Watch Directory��6=   $
��Left=�6W   $��27��6r   $��	19,10,0,0q�6~   $	��Top=�6�   5      �� 5   
   -   5         ��Name$ IncludeSubfoldersCheckBox��5      $��Include Subfolders��66   $
��Left=�6S   $��
6.8,34,0,0q�6n   $	��Top=�6�   $��27��6�   $��1��6�   �� 5   
   -   5         ��Name$ SelectDirectoryLabel��5      $��Select Directory��6.   $
��Left=�6I   $��	10,75,0,0q�6d   $	��Top=�6w    J� RenderTransformOrigin$ 0.078,-0.047&�6�   $	��176��6�   $
��Bold"�6�   �� 5   
   -   5         ��Name$ SelectDirectoryTextBox��5      $
��Left=�62   $��23��6M   $��
10,106,0,0q�6Y    �� TextWrapping$
 Wrap=�6�   $	��Top=�6�   $	��571��6�   $��2��6�   ��6m   �� 	 �� XPath$	 DirectoryPath��
 �� UpdateSourceTrigger$
 PropertyChanged=�6�   �� 5   
   -   5         ��Name$ BrowseButton��5      $��Browse��6'   $
��Left=�68   $��343.8,106,0,0q�6S   $	��Top=�6j   $��85��6�   $ -0.387,-0.979&�6�   $��23��6�   $��1��6�   �� 5   
   -   5        $ StartWatchingButton��5      $��Start Watching��6.   $
��Left=�6G   $��
10,143,0,0q�6b   $	��Top=�6v   $	��126��6�   �� 5   
   -   5        $ WatchingLogLabel��5      $��Watching Log��6*   $
��Left=�6A   $��
10,188,0,0q�6\   $	��Top=�6p   $ 0.078,-0.047&�6�   $	��176��6�   $
��Bold"�6�   $y�Hidden=�6�   �� 5   
   -	   5        $ WatchingLogTextBox��5      $
��Left=�6.   $	��264��6I   $��
10,219,0,0q�6V   $
 Wrap=�6j    �� AcceptsReturn . " �� VerticalScrollBarVisibility$ Visible=�6�   $	��Top=�6�   $	��760��6�   $��2��6�   $y�Hidden=�6�   �� 5   
   -
   5        $ button��5      $��Button��6!   $��1��62   $
��Left=�6B   $��114.8,162,0,0q�6]   $	��Top=�6t   $��75��6�   $y�Hidden=�6�   5      5      