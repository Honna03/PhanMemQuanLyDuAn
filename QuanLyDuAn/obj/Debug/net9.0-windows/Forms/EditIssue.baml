   M S B A M L   `   `   `  ����E  AQuanLyDuAn, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:QuanLyDuAn.FormsQuanLyDuAn.Forms  5           QuanLyDuAn.Forms.EditIssue   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0localclr-namespace:QuanLyDuAn.Forms     5      ���#F5F5F5.��5	      � $��20q�6     � RowDefinitions  5
   
   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� -   5         ��Name$ 
IssuesList��5      $��0��6'   ���#E0E0E0 �� SelectionMode$ Single=�6J    �� View 5      �� 5      .P�5      �� $��Mô tả��6%    �� Width$	 350��64    �� DisplayMemberBinding 6@   �� *Description+�� 5      $��Mức độ��6%   $	 150��65    6A   �� *
Severity+5      5      �� 5   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   $��0,10,0,0q�6W   .+�5      �� -   5        $��Thêm��5      $	��100��6$   $��30��60   $��0,0,10,0q�6<   ���#2196F3
�������$��0q�6v   ��5      �� 5       �� 
TargetType" ��6   .�5      ��  �� Property � ��	 �� Value	 5      �� 5    "   
 �� 
TargetType"
 ��62   .R�5!   &   �� #��z��6-    �� CornerRadius$ 5��6W   ._�5"   *   �� $��Center=�6;   $��Center=�6X   5#   '   5$   #   5&       �� Triggers 5'      P� 5(       P� Property �@J�IsMouseOver P� Value$
 True��6=   . �5)   "   ��  � ��	 �#1976D26?   5*      5,      5.      �� 5/      -   5        $��Sửa��5/      $	��100��6#   $��30��6/   $��0,0,10,0q�6;   ���#FFC107
�������$��0q�6u   ��50      �� 51      " ��6   .�52      ��  � ��	 53      �� 54   "   "
 ��62   .R�55   &   �� #��z��6-   $ 5��6W   ._�56   *   �� $��Center=�6;   $��Center=�6X   57   '   58   #   5:       5;      P� 5<       �@J�IsMouseOver$
 True��6=   . �5=   "   ��  � ��	 �#FFA0006?   5>      5@      5B      �� 5C      -   5        $
��Xóa��5C      $	��100��6#   $��30��6/   ���#F44336
�������$��0q�6c   ��5D      �� 5E      " ��6   .�5F      ��  � ��	 5G      �� 5H   "   "
 ��62   .R�5I   &   �� #��z��6-   $ 5��6W   ._�5J   *   �� $��Center=�6;   $��Center=�6X   5K   '   5L   #   5N       5O      P� 5P       �@J�IsMouseOver$
 True��6=   . �5Q   "   ��  � ��	 �#D32F2F6?   5R      5T      5V      5W      5X      5Y      